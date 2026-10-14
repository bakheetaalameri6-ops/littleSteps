"""Run against a disposable seeded demo database; creates visits, exits and audit rows.

Usage: python3 tests/gate_audit_smoke.py [http://localhost:7001]
Requires the four seeded accounts with their original demo passwords.
"""
import datetime
import json
import sys
import urllib.error
import urllib.request
from zoneinfo import ZoneInfo

BASE = (sys.argv[1] if len(sys.argv) > 1 else 'http://localhost:7001') + '/api'


def call(path, method='GET', body=None, token=None, status=200):
    headers = {'Content-Type': 'application/json'}
    if token:
        headers['Authorization'] = 'Bearer ' + token
    request = urllib.request.Request(BASE + path, method=method, headers=headers,
                                    data=json.dumps(body).encode() if body is not None else None)
    try:
        response = urllib.request.urlopen(request)
    except urllib.error.HTTPError as error:
        response = error
    raw = response.read()
    assert response.code == status, (path, response.code, raw.decode())
    return json.loads(raw) if raw else None


def login(username):
    return call('/auth/login', 'POST', {'username': username, 'password': 'Demo@2026'})['token']


admin, teacher, guard, manager = map(login, ['bakheeta', 'r.saleh', 'a.karim', 'n.alzaabi'])
for token in [admin, teacher, manager]:
    call('/gate/visits', token=token, status=403)
for token in [admin, teacher, guard]:
    call('/audit', token=token, status=403)
call('/gate/visits', status=401)


def gate_audits():
    return call('/audit?area=Gate%20control&pageSize=100', token=manager)


before = gate_audits()['total']
visit = call('/gate/visits', 'POST', {'fullName': 'Integration Visitor',
             'emiratesId': '784-1990-1234567-1', 'reason': 'Meeting',
             'personVisited': 'Administration'}, guard)
assert any(v['id'] == visit['id'] and v['timeOut'] is None
           for v in call('/gate/visits', token=guard)['items'])
call('/gate/visits/' + str(visit['id']) + '/checkout', 'PUT', token=guard, status=204)
call('/gate/visits/' + str(visit['id']) + '/checkout', 'PUT', token=guard, status=409)
assert gate_audits()['total'] == before + 2

today = datetime.datetime.now(ZoneInfo('Asia/Dubai')).date().isoformat()
sheet = call('/attendance?date=' + today, token=teacher)
assert not sheet['weekend'], 'Run attendance/early-exit checks on a school day.'
student_id = sheet['students'][0]['student']['id']
detail = call('/students/' + str(student_id), token=admin)
adult = detail['guardians'][0]['emiratesId']
unknown = '784-1900-0000000-0'
assert all(g['emiratesId'] != unknown for g in detail['guardians'])
before = gate_audits()['total']
exits_before = len(call('/gate/early-exits', token=guard))
result = call('/gate/early-exits/check', 'POST', {'studentId': student_id, 'emiratesId': unknown}, guard)
assert not result['authorized']
assert len(call('/gate/early-exits', token=guard)) == exits_before + 1
assert gate_audits()['total'] == before + 1
assert gate_audits()['items'][0]['result'] == 'Refused'
result = call('/gate/early-exits/check', 'POST', {'studentId': student_id, 'emiratesId': adult}, guard)
assert result['authorized']
assert gate_audits()['total'] == before + 1
marks = [{'studentId': s['student']['id'], 'status': 'P'} for s in sheet['students']]
call('/attendance', 'PUT', {'date': today, 'marks': marks, 'correction': sheet['saved']}, teacher, 204)
call('/gate/early-exits', 'POST', {'studentId': student_id, 'emiratesId': adult, 'reason': 'Family reason'}, guard)
call('/gate/early-exits', 'POST', {'studentId': student_id, 'emiratesId': adult, 'reason': 'Family reason'}, guard, 409)
assert call('/students/' + str(student_id), token=guard)['status'] == 'Left early'
assert gate_audits()['total'] == before + 2
assert all(g['emiratesId'] is None and g['phone'] is None
           for g in call('/students/' + str(student_id), token=guard)['guardians'])
page = call('/audit?area=Gate%20control&pageSize=1&page=2', token=manager)
assert len(page['items']) == 1 and page['page'] == 2
assert all(e['area'] == 'Gate control' for e in page['items'])
assert call('/audit/users', token=manager)
assert call('/audit?from=' + today + '&to=' + today, token=manager)['total'] > 0
call('/audit?from=2026-10-15&to=2026-10-01', token=manager, status=400)
print('PASS: gate permissions, visitor lifecycle, immediate refusal, linked check, exit, duplicate protection, privacy, audit counts and paging')
