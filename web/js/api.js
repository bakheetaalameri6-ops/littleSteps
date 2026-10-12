async function api(path, options = {}) {
  const headers = new Headers(options.headers || {});
  const token = sessionStorage.getItem('token');
  if (token) headers.set('Authorization', `Bearer ${token}`);
  if (options.body && !(options.body instanceof FormData)) {
    headers.set('Content-Type', 'application/json');
    options.body = JSON.stringify(options.body);
  }
  const response = await fetch(API_BASE + path, {...options, headers});
  if (response.status === 401 && path !== '/auth/login') {
    sessionStorage.clear(); location.href = 'login.html'; throw new Error('Please sign in again.');
  }
  if (!response.ok) {
    const data = await response.json().catch(() => ({}));
    throw new Error(data.message || data.title || Object.values(data.errors || {}).flat().join(' ') || 'The request could not be completed.');
  }
  if (response.status === 204) return null;
  return response.json();
}
const escapeHtml = value => String(value ?? '').replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const schoolToday = () => new Intl.DateTimeFormat('en-CA', {timeZone:'Asia/Dubai',year:'numeric',month:'2-digit',day:'2-digit'}).format(new Date());
const timeText = value => value ? new Date(value).toLocaleTimeString('en-GB',{timeZone:'Asia/Dubai',hour:'2-digit',minute:'2-digit'}) : '—';
function notify(message) { const toast = document.createElement('div'); toast.id='toast'; toast.setAttribute('role','status'); toast.textContent=message; document.querySelector('#toast')?.remove(); document.body.append(toast); setTimeout(()=>toast.remove(),5000); }
function showError(error) { const el=document.querySelector('#error'); if(el){el.textContent=error.message;el.className='notice danger error-banner';el.setAttribute('role','alert');el.scrollIntoView({block:'nearest'});} }
function handle(fn) { return async event => {event?.preventDefault(); document.querySelector('#error')?.replaceChildren();try {await fn(event);} catch(error){showError(error);} }; }
// Photos require the same authenticated request as other student information.
async function loadPhotos(root=document) {
  for(const img of root.querySelectorAll('img[data-photo]')) {
    try {const r=await fetch(API_BASE+img.dataset.photo,{headers:{Authorization:`Bearer ${sessionStorage.getItem('token')}`}});if(!r.ok)throw new Error();const url=URL.createObjectURL(await r.blob());img.src=url;img.onload=()=>URL.revokeObjectURL(url);} catch{img.replaceWith(photoPlaceholder(img.classList.contains('large')));}
  }
}
function photoPlaceholder(large=false){const el=document.createElement('span');el.className='photo missing'+(large?' large':'');el.setAttribute('aria-label','Photo missing');el.innerHTML='<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 6h4l2-3h6l2 3h4v15H3z"/><circle cx="12" cy="13" r="4"/></svg>'+(large?'Photo missing':'');return el;}
function photo(person,large=false){return person.photoUrl?`<img class="photo${large?' large':''}" data-photo="${escapeHtml(person.photoUrl)}" alt="Photo of ${escapeHtml(person.fullName)}">`:photoPlaceholder(large).outerHTML;}
function identity(person,large=false){return `<div class="identity">${photo(person,large)}<div><strong>${escapeHtml(person.fullName)}</strong><small class="mono">${escapeHtml(person.studentCode||'')}</small>${person.className?`<small> · ${escapeHtml(person.className)}</small>`:''}</div></div>`;}
function chip(text){return `<span class="chip ${['OK','Complete','Authorized','Present','Active'].includes(text)?'ok':['Refused','Absent','Inactive'].includes(text)?'danger':['Incomplete','Photo missing — incomplete','Not saved yet'].includes(text)?'warn':''}">${escapeHtml(text)}</span>`;}
function table(id,headers,rows){document.querySelector(id).innerHTML=`<table class="table" id="${id.slice(1)}-table"><thead><tr>${headers.map(h=>`<th>${h}</th>`).join('')}</tr></thead><tbody>${rows.map(r=>`<tr>${r.map(c=>`<td>${c}</td>`).join('')}</tr>`).join('')}</tbody></table>`;if(window.jQuery?.fn.DataTable)jQuery(`${id}-table`).DataTable({pageLength:10,order:[],language:{emptyTable:'No records to show.'}});loadPhotos(document.querySelector(id));}
