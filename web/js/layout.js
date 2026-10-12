const landing={Admin:'dashboard.html',Teacher:'attendance.html',SecurityGuard:'early-exit.html',Manager:'audit-log.html'};
const roleLabel={Admin:'Administration',Teacher:'Teacher',SecurityGuard:'Security guard',Manager:'School management'};
let me;
async function layout(title,area){
  me=await api('/auth/me');if(me.mustChangePassword){location.href='change-password.html';return false;}
  const menus={Admin:[['',[['Dashboard','dashboard.html']]],['Staff & Classes',[['Staff','staff.html'],['Classes','classes.html']]],['Students & Guardians',[['Students','students.html'],['Enrol student','student-form.html'],['Guardians','guardian-form.html'],['Class transfer','class-transfer.html']]]],Teacher:[['Attendance',[['My class','attendance.html']]]],SecurityGuard:[['Gate control',[['Visitors','visitors.html'],['Early exit','early-exit.html']]]],Manager:[['Audit',[['Audit log','audit-log.html']]]]};
  const current=location.pathname.split('/').pop();const allowed=menus[me.role].flatMap(g=>g[1].map(i=>i[1]));if(!allowed.includes(current)){location.href=landing[me.role];return false;}
  document.querySelector('#sidebar').innerHTML='<div class="brand"><span class="logo">LS</span>LittleSteps</div><nav aria-label="Main">'+menus[me.role].map(([group,links])=>`${group?`<div class="nav-group">${group}</div>`:''}${links.map(([label,url])=>`<a class="${current===url?'active':''}" href="${url}">${label}</a>`).join('')}`).join('')+'</nav>';
  document.querySelector('#header').innerHTML=`<div><div class="eyebrow">${escapeHtml(area)}</div><h1>${escapeHtml(title)}</h1></div><div class="user-box"><div><strong>${escapeHtml(me.fullName)}</strong><div class="muted">${escapeHtml(roleLabel[me.role])}${me.className?' · '+escapeHtml(me.className):''}</div></div><span class="avatar">${escapeHtml(me.fullName.split(' ').map(x=>x[0]).slice(0,2).join(''))}</span><button id="logout" class="btn btn-outline-secondary">Log out</button></div>`;
  document.querySelector('#logout').onclick=handle(async()=>{await api('/auth/logout',{method:'POST'});sessionStorage.clear();location.href='login.html';});return true;
}
