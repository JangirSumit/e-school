import { useEffect, useState } from 'react';

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || '').trim();

if (!API_BASE_URL &&
  window.location.hostname === 'localhost' &&
  window.location.port === '5001' &&
  window.location.protocol === 'http:') {
  window.location.replace(`https://localhost:7001${window.location.pathname}${window.location.search}${window.location.hash}`);
}

const marketingHighlights = [
  {
    title: 'Student Management',
    accent: 'Records',
    items: [
      'Student profiles and admission records',
      'Class and section-wise organization',
      'Parent-linked student access'
    ]
  },
  {
    title: 'Academic Operations',
    accent: 'Daily Work',
    items: [
      'Class setup and staff management',
      'Holiday and school calendar visibility',
      'Simple web and mobile workflows'
    ]
  },
  {
    title: 'Connected Experience',
    accent: 'School Community',
    items: [
      'Teacher, parent, and student access',
      'Role-based dashboards after login',
      'Shared school experience across devices'
    ]
  }
];

const benefitTiles = [
  { title: 'Web + Mobile Access', summary: 'Use the platform from the school office and on mobile with the same connected experience.' },
  { title: 'Role-Based Experience', summary: 'School admins, faculty, parents, and students see the right information for their work.' },
  { title: 'Clear School Records', summary: 'Keep academic and administrative records organized in one place.' },
  { title: 'Simple Daily Use', summary: 'Designed to keep everyday school operations clear, quick, and easy to follow.' }
];

const stats = [
  { value: '1', label: 'platform for school operations' },
  { value: '2', label: 'ways to access: web and mobile' },
  { value: '5', label: 'core user groups supported' }
];

const trustPoints = [
  'Designed for schools, teachers, parents, and students',
  'Works on web and mobile',
  'Built for day-to-day school operations'
];

const heroSignals = [
  'Admissions',
  'Attendance',
  'Faculty',
  'Parents',
  'Reports',
  'Web + Mobile'
];

const testimonials = [
  {
    quote: 'A cleaner way to handle school operations without juggling spreadsheets and scattered chats.',
    name: 'For School Leaders',
    meta: 'Operations-first workflow'
  },
  {
    quote: 'Designed so office teams can work from the web while parents and staff stay connected anywhere.',
    name: 'For Growing Schools',
    meta: 'Web plus mobile access'
  }
];

const emptyPublicInfo = {
  appName: 'eSchool',
  tagline: 'One welcoming app for school leaders, teachers, parents, and students.',
  supportEmail: 'support@eschool.app',
  supportPhone: '+91 98765 43210',
  supportWhatsApp: '+91 98765 43210',
  helpMessage: 'Need your school onboarded? Contact support and we will set up your school, admin access, and login code.',
  loginHint: 'Owner uses email. School users log in with SCHOOLCODE@username.'
};

const initialLogin = { email: '', password: '' };
const initialOwnerSchool = {
  schoolName: '',
  email: '',
  phone: '',
  address: '',
  contactPersonName: '',
  adminName: '',
  adminUsername: '',
  adminPassword: ''
};
const initialClass = { name: '', section: '', capacity: '30', classTeacherUserId: '' };
const initialFaculty = {
  fullName: '',
  username: '',
  email: '',
  phone: '',
  employeeId: '',
  department: '',
  qualification: '',
  joiningDate: new Date().toISOString().slice(0, 10),
  password: ''
};
const initialStudent = {
  username: '',
  fullName: '',
  email: '',
  phone: '',
  rollNumber: '',
  class: '',
  section: '',
  dateOfBirth: '',
  parentName: '',
  parentUsername: '',
  parentEmail: '',
  parentPhone: '',
  parentPassword: '',
  password: ''
};
const initialHoliday = { name: '', date: '', description: '' };
const initialSchoolEditor = {
  id: '',
  schoolName: '',
  schoolCode: '',
  contactPersonName: '',
  email: '',
  phone: '',
  address: '',
  isActive: true,
  subscriptionPlan: 'Standard',
  billingCycle: 'Yearly',
  billingAmount: '0',
  subscriptionStart: '',
  subscriptionEnd: '',
  lastBillingDate: '',
  nextBillingDate: '',
  licenseSeats: '100',
  licensedModules: '',
  billingNotes: '',
  studentCount: 0,
  facultyCount: 0,
  classCount: 0,
  createdAt: ''
};

async function api(path, options = {}, token) {
  const storedToken = window.sessionStorage.getItem('eschool-web-token') || '';
  const authToken = token || storedToken;
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {})
  };

  if (authToken) {
    headers.Authorization = `Bearer ${authToken}`;
  }

  let response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...options,
      headers
    });
  } catch {
    const error = new Error('Unable to reach the server. Make sure the API is running and the web app is pointing to it.');
    error.status = 0;
    throw error;
  }

  if (!response.ok) {
    const contentType = response.headers.get('content-type') || '';
    let message = '';

    if (contentType.includes('application/json')) {
      const payload = await response.json();
      message = payload.message || payload.title || JSON.stringify(payload);
    } else {
      message = await response.text();
    }

    const error = new Error(message || `Request failed with status ${response.status}`);
    error.status = response.status;
    throw error;
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

function App() {
  const [publicInfo, setPublicInfo] = useState(emptyPublicInfo);
  const [loginForm, setLoginForm] = useState(initialLogin);
  const [isLoginOpen, setIsLoginOpen] = useState(false);
  const [token, setToken] = useState('');
  const [session, setSession] = useState(null);
  const [dashboard, setDashboard] = useState(null);
  const [holidays, setHolidays] = useState([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [ownerSchoolForm, setOwnerSchoolForm] = useState(initialOwnerSchool);
  const [classForm, setClassForm] = useState(initialClass);
  const [facultyForm, setFacultyForm] = useState(initialFaculty);
  const [studentForm, setStudentForm] = useState(initialStudent);
  const [holidayForm, setHolidayForm] = useState(initialHoliday);
  const [publicPage, setPublicPage] = useState(window.location.hash === '#features' ? 'features' : 'home');
  const [ownerModule, setOwnerModule] = useState('overview');
  const [schoolEditor, setSchoolEditor] = useState(initialSchoolEditor);

  useEffect(() => {
    loadPublicInfo();

    const storedToken = window.sessionStorage.getItem('eschool-web-token');
    const storedSession = window.sessionStorage.getItem('eschool-web-session');

    if (storedToken) {
      setToken(storedToken);
    }

    if (storedSession) {
      try {
        setSession(JSON.parse(storedSession));
      } catch {
        window.sessionStorage.removeItem('eschool-web-session');
      }
    }

    function syncPublicPage() {
      setPublicPage(window.location.hash === '#features' ? 'features' : 'home');
    }

    window.addEventListener('hashchange', syncPublicPage);
    return () => window.removeEventListener('hashchange', syncPublicPage);
  }, []);

  useEffect(() => {
    if (token) {
      loadDashboard(token);
    }
  }, [token]);

  useEffect(() => {
    if (!dashboard || dashboard.role !== 'Owner') {
      return;
    }

    if (dashboard.schools.length === 0) {
      setSchoolEditor(initialSchoolEditor);
      return;
    }

    const currentSchool = dashboard.schools.find((school) => school.id === schoolEditor.id) || dashboard.schools[0];
    setSchoolEditor(mapSchoolToEditor(currentSchool));
  }, [dashboard]);

  async function loadPublicInfo() {
    try {
      const info = await api('/api/public/app-info');
      setPublicInfo(info);
    } catch {
      setPublicInfo(emptyPublicInfo);
    }
  }

  async function loadDashboard(authToken = token) {
    if (!authToken) {
      return;
    }

    try {
      setLoading(true);
      setError('');
      const data = await api('/api/management/dashboard', {}, authToken);
      setDashboard(data);
      if (['SchoolAdmin', 'Parent', 'Student'].includes(data.role)) {
        try {
          const holidayData = await api('/api/holidays', {}, authToken);
          setHolidays(holidayData);
        } catch {
          setHolidays([]);
        }
      } else {
        setHolidays([]);
      }
    } catch (err) {
      if (err.status === 401) {
        clearSession();
        setError('Your session is not active. Please sign in again.');
        return;
      }

      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  async function handleLogin(event) {
    event.preventDefault();
    try {
      setLoading(true);
      setError('');
      setMessage('');
      const response = await api('/api/auth/login', {
        method: 'POST',
        body: JSON.stringify(loginForm)
      });

      window.sessionStorage.setItem('eschool-web-token', response.token);
      window.sessionStorage.setItem('eschool-web-session', JSON.stringify(response));
      setToken(response.token);
      setSession(response);
      setIsLoginOpen(false);
      setMessage(`Signed in as ${response.fullName}`);
    } catch (err) {
      setError('Login failed. Please check your credentials or contact support for onboarding.');
    } finally {
      setLoading(false);
    }
  }

  async function logout() {
    try {
      await api('/api/auth/logout', { method: 'POST' });
    } catch {
      // Clear local session even if the server-side cookie delete fails.
    }

    clearSession();
    setMessage('Logged out.');
    setError('');
  }

  function clearSession() {
    window.sessionStorage.removeItem('eschool-web-token');
    window.sessionStorage.removeItem('eschool-web-session');
    setToken('');
    setSession(null);
    setDashboard(null);
    setHolidays([]);
  }

  async function createOwnerSchool(event) {
    event.preventDefault();
    try {
      setLoading(true);
      await api('/api/management/schools', {
        method: 'POST',
        body: JSON.stringify(ownerSchoolForm)
      }, token);
      setOwnerSchoolForm(initialOwnerSchool);
      setMessage('School onboarded successfully.');
      await loadDashboard();
    } catch (err) {
      setError('Unable to onboard school.');
    } finally {
      setLoading(false);
    }
  }

  async function toggleSchoolStatus(school) {
    try {
      setLoading(true);
      await api(`/api/management/schools/${school.id}/status`, {
        method: 'PUT',
        body: JSON.stringify({ isActive: !school.isActive })
      }, token);
      await loadDashboard();
    } catch {
      setError('Unable to update school status.');
    } finally {
      setLoading(false);
    }
  }

  async function saveSchoolManagement(event) {
    event.preventDefault();
    if (!schoolEditor.id) {
      return;
    }

    try {
      setLoading(true);
      await api(`/api/management/schools/${schoolEditor.id}`, {
        method: 'PUT',
        body: JSON.stringify({
          contactPersonName: schoolEditor.contactPersonName,
          email: schoolEditor.email,
          phone: schoolEditor.phone,
          address: schoolEditor.address,
          isActive: schoolEditor.isActive,
          subscriptionPlan: schoolEditor.subscriptionPlan,
          billingCycle: schoolEditor.billingCycle,
          billingAmount: Number(schoolEditor.billingAmount || 0),
          subscriptionStart: schoolEditor.subscriptionStart,
          subscriptionEnd: schoolEditor.subscriptionEnd,
          lastBillingDate: schoolEditor.lastBillingDate || null,
          nextBillingDate: schoolEditor.nextBillingDate || null,
          licenseSeats: Number(schoolEditor.licenseSeats || 0),
          licensedModules: schoolEditor.licensedModules,
          billingNotes: schoolEditor.billingNotes
        })
      }, token);
      setMessage(`Updated ${schoolEditor.schoolName}.`);
      await loadDashboard();
    } catch {
      setError('Unable to update school details.');
    } finally {
      setLoading(false);
    }
  }

  async function createClass(event) {
    event.preventDefault();
    try {
      setLoading(true);
      await api('/api/management/classes', {
        method: 'POST',
        body: JSON.stringify({
          ...classForm,
          capacity: Number(classForm.capacity),
          classTeacherUserId: classForm.classTeacherUserId || null
        })
      }, token);
      setClassForm(initialClass);
      setMessage('Class created.');
      await loadDashboard();
    } catch {
      setError('Unable to create class.');
    } finally {
      setLoading(false);
    }
  }

  async function createFaculty(event) {
    event.preventDefault();
    try {
      setLoading(true);
      await api('/api/faculties', {
        method: 'POST',
        body: JSON.stringify(facultyForm)
      }, token);
      setFacultyForm(initialFaculty);
      setMessage('Faculty member added.');
      await loadDashboard();
    } catch {
      setError('Unable to create faculty account.');
    } finally {
      setLoading(false);
    }
  }

  async function createStudent(event) {
    event.preventDefault();
    try {
      setLoading(true);
      await api('/api/students', {
        method: 'POST',
        body: JSON.stringify(studentForm)
      }, token);
      setStudentForm(initialStudent);
      setMessage('Student and parent accounts created.');
      await loadDashboard();
    } catch {
      setError('Unable to create student record.');
    } finally {
      setLoading(false);
    }
  }

  async function createHoliday(event) {
    event.preventDefault();
    try {
      setLoading(true);
      await api('/api/holidays', {
        method: 'POST',
        body: JSON.stringify(holidayForm)
      }, token);
      setHolidayForm(initialHoliday);
      setMessage('Holiday added.');
      await loadDashboard();
    } catch {
      setError('Unable to add holiday.');
    } finally {
      setLoading(false);
    }
  }

  let greeting = publicInfo.tagline;
  if (dashboard) {
    const school = dashboard.schoolCode ? `${dashboard.schoolName} (${dashboard.schoolCode})` : dashboard.schoolName;
    greeting = `${dashboard.userName} • ${dashboard.role}${school ? ` • ${school}` : ''}`;
  }

  const isSignedIn = Boolean(token || session);
  const selectedSchool = dashboard?.role === 'Owner'
    ? dashboard.schools.find((school) => school.id === schoolEditor.id) || dashboard.schools[0] || null
    : null;

  return (
    <div className="app-shell">
      <div className="ambient ambient-one" />
      <div className="ambient ambient-two" />
      <div className="grid-wash" />
      <main className="page">
        {!isSignedIn && (
          <header className="topbar">
            <div className="brand-lockup">
              <div className="brand-mark">eS</div>
              <div>
                <strong>{publicInfo.appName}</strong>
                <span>School management platform</span>
              </div>
            </div>

            <nav className="topnav">
              <a href="#">Home</a>
              <a href="#features">Features</a>
              <a href="#contact">Onboarding</a>
              <button className="nav-login" type="button" onClick={() => setIsLoginOpen(true)}>Login</button>
            </nav>
          </header>
        )}

        {!isSignedIn ? (
          <>
            {publicPage === 'home' ? (
              <>
                <section className="hero-card marketing-hero">
                  <div className="hero-copy">
                    <span className="hero-kicker">School management platform</span>
                    <h1>{publicInfo.appName}</h1>
                    <h2 className="hero-title">A modern school experience for administrators, teachers, parents, and students.</h2>
                    <p className="hero-text">{publicInfo.tagline}</p>
                    <p className="hero-help">
                      Bring admissions, class records, school communication, and daily operations into one elegant system built for schools.
                    </p>
                    <div className="hero-actions">
                      <button className="primary" type="button" onClick={() => setIsLoginOpen(true)}>Open Login</button>
                      <a href="#features" className="ghost cta-link">View Features</a>
                    </div>
                  </div>

                  <div className="hero-side-panel">
                    <div className="school-scene compact-scene">
                      <div className="scene-orb orb-one" />
                      <div className="scene-orb orb-two" />
                      <div className="school-card main-school-card">
                        <span className="school-tag">School Day</span>
                        <strong>Attendance, classes, and parent updates in one flow.</strong>
                      </div>
                      <div className="school-card floating-school-card">
                        <span>School Notice Board</span>
                        <strong>Events, holidays, and classroom highlights</strong>
                      </div>
                    </div>

                    <div className="support-panel dark-support" id="contact">
                      <strong>Talk to us for onboarding</strong>
                      <span>Contact us for setup, pricing, and support details.</span>
                      <span>Email: {publicInfo.supportEmail}</span>
                      <span>Call: {publicInfo.supportPhone}</span>
                      <span>WhatsApp: {publicInfo.supportWhatsApp}</span>
                    </div>
                  </div>
                </section>

                <section className="visual-band">
                  <article className="floating-callout">
                    <span className="section-accent">Web + Mobile</span>
                    <h3>Marketing highlights and the mobile experience are now part of the home page.</h3>
                    <p className="hero-help">
                      School leaders can explore the product story before login, while families and staff still have a clear path into their role-based workspace.
                    </p>
                    <div className="hero-pills">
                      {trustPoints.map((point) => (
                        <span key={point}>{point}</span>
                      ))}
                    </div>
                  </article>

                  <article className="phone-showcase">
                    <div className="phone-frame">
                      <div className="phone-notch" />
                      <div className="phone-screen">
                        <div className="phone-header">
                          <div>
                            <div className="phone-app-name">{publicInfo.appName}</div>
                            <div className="phone-date">Mobile snapshot</div>
                          </div>
                          <span className="section-accent">Live School</span>
                        </div>

                        <div className="phone-card hero-phone-card">
                          <strong>Today at a glance</strong>
                          <span>Classes, parent updates, student records, and staff actions in one place.</span>
                        </div>

                        <div className="phone-metrics">
                          {stats.slice(0, 2).map((item) => (
                            <div className="phone-card" key={item.label}>
                              <strong>{item.value}</strong>
                              <span>{item.label}</span>
                            </div>
                          ))}
                        </div>

                        <div className="phone-list">
                          {heroSignals.map((signal) => (
                            <div className="phone-list-item" key={signal}>{signal}</div>
                          ))}
                        </div>
                      </div>
                    </div>
                  </article>
                </section>

                <section className="showcase-grid feature-showcase-page">
                  {marketingHighlights.map((section) => (
                    <article className="feature-showcase" key={section.title}>
                      <span className="section-accent">{section.accent}</span>
                      <h3>{section.title}</h3>
                      <div className="mini-list">
                        {section.items.map((item) => (
                          <div className="mini-item feature-item" key={item}>{item}</div>
                        ))}
                      </div>
                    </article>
                  ))}
                </section>
              </>
            ) : (
              <section className="feature-page">
                <div className="feature-page-header">
                  <span className="hero-kicker">Features</span>
                  <h1>{publicInfo.appName}</h1>
                  <p className="hero-help">Explore the school-focused product features available across web and mobile.</p>
                </div>

                <section className="showcase-grid feature-showcase-page">
                  {marketingHighlights.map((section) => (
                    <article className="feature-showcase" key={section.title}>
                      <span className="section-accent">{section.accent}</span>
                      <h3>{section.title}</h3>
                      <div className="mini-list">
                        {section.items.map((item) => (
                          <div className="mini-item feature-item" key={item}>{item}</div>
                        ))}
                      </div>
                    </article>
                  ))}
                </section>

                <section className="role-snapshot-grid">
                  {benefitTiles.map((item) => (
                    <article className="role-snapshot" key={item.title}>
                      <h4>{item.title}</h4>
                      <p>{item.summary}</p>
                    </article>
                  ))}
                </section>

                <section className="testimonial-grid">
                  {testimonials.map((item) => (
                    <article className="testimonial-card" key={item.name}>
                      <p className="quote">"{item.quote}"</p>
                      <strong>{item.name}</strong>
                      <span>{item.meta}</span>
                    </article>
                  ))}
                </section>
              </section>
            )}
          </>
        ) : (
          <section className="hero-card app-hero">
            <div className="hero-copy">
              <span className="hero-kicker">Signed-in Workspace</span>
              <h1>{publicInfo.appName}</h1>
              <h2 className="hero-title">{greeting}</h2>
              <p className="hero-help">
                Your dashboard below shows role-based school data and actions from the live application.
              </p>
            </div>

            <div className="auth-card signed-in">
              <h2>{session?.role || 'User'} Workspace</h2>
              <p className="muted">
                Refresh your dashboard to load the latest data for your role, or sign out to return to the public site.
              </p>
              <button className="primary" onClick={() => loadDashboard()} disabled={loading}>Refresh Dashboard</button>
              <button className="ghost" onClick={logout} type="button">Logout</button>
            </div>
          </section>
        )}

        {!isSignedIn && isLoginOpen && (
          <div className="dialog-backdrop" onClick={() => setIsLoginOpen(false)}>
            <div className="login-dialog" onClick={(event) => event.stopPropagation()}>
              <div className="dialog-header">
                <div>
                  <span className="hero-kicker">Secure Login</span>
                  <h3>Access your school workspace</h3>
                </div>
                <button className="dialog-close" type="button" onClick={() => setIsLoginOpen(false)}>×</button>
              </div>

              <form className="stack" onSubmit={handleLogin}>
                <p className="muted">{publicInfo.loginHint}</p>
                <input placeholder="owner@eschool.app or ABCD@admin" value={loginForm.email} onChange={(e) => setLoginForm({ ...loginForm, email: e.target.value })} />
                <input type="password" placeholder="Password" value={loginForm.password} onChange={(e) => setLoginForm({ ...loginForm, password: e.target.value })} />
                <button className="primary" disabled={loading}>Sign In</button>
              </form>
            </div>
          </div>
        )}

        {(message || error) && (
          <section className={`notice ${error ? 'error' : 'success'}`}>
            {error || message}
          </section>
        )}

        {dashboard && (
          <>
            <section className="metrics-grid">
              {dashboard.metrics.map((metric) => (
                <article className="metric-card" key={metric.label}>
                  <span>{metric.label}</span>
                  <strong>{metric.value}</strong>
                  <p>{metric.hint}</p>
                </article>
              ))}
            </section>

            <section className="dashboard-grid">
              <article className="panel wide">
                <h3>Recommended Actions</h3>
                <div className="list-grid">
                  {dashboard.highlights.map((item) => (
                    <div className="list-card" key={item.title}>
                      <strong>{item.title}</strong>
                      <p>{item.description}</p>
                    </div>
                  ))}
                </div>
              </article>

              {dashboard.role === 'Owner' && (
                <>
                  <article className="panel wide owner-shell">
                    <div className="owner-module-tabs">
                      {[
                        ['overview', 'Overview'],
                        ['schools', 'Schools'],
                        ['billing', 'Billing & Licensing'],
                        ['onboarding', 'Onboarding']
                      ].map(([value, label]) => (
                        <button
                          key={value}
                          type="button"
                          className={ownerModule === value ? 'primary' : 'ghost'}
                          onClick={() => setOwnerModule(value)}
                        >
                          {label}
                        </button>
                      ))}
                    </div>

                    {ownerModule === 'overview' && (
                      <div className="owner-workspace">
                        <div className="list-grid">
                          {dashboard.schools.slice(0, 4).map((school) => (
                            <div className="list-card" key={school.id}>
                              <strong>{school.schoolName}</strong>
                              <p>{school.schoolCode} • {school.subscriptionPlan} • {school.billingCycle}</p>
                              <p>{school.isActive ? 'Active tenant' : 'Inactive tenant'} • {school.licenseSeats} seats</p>
                            </div>
                          ))}
                        </div>
                        <div className="two-col">
                          <div className="panel owner-subpanel">
                            <h4>Portfolio Snapshot</h4>
                            <div className="mini-list">
                              <div className="mini-item">Active schools: {dashboard.schools.filter((school) => school.isActive).length}</div>
                              <div className="mini-item">Renewals due this quarter: {dashboard.schools.filter((school) => isDateWithinDays(school.nextBillingDate, 90)).length}</div>
                              <div className="mini-item">Total licensed seats: {dashboard.schools.reduce((sum, school) => sum + school.licenseSeats, 0)}</div>
                            </div>
                          </div>
                          <div className="panel owner-subpanel">
                            <h4>Revenue Visibility</h4>
                            <div className="mini-list">
                              <div className="mini-item">Annualized billing: {formatMoney(dashboard.schools.reduce((sum, school) => sum + Number(school.billingAmount || 0), 0))}</div>
                              <div className="mini-item">Schools pending next bill: {dashboard.schools.filter((school) => isDateWithinDays(school.nextBillingDate, 30)).length}</div>
                              <div className="mini-item">Plans in use: {[...new Set(dashboard.schools.map((school) => school.subscriptionPlan))].join(', ') || 'None'}</div>
                            </div>
                          </div>
                        </div>
                      </div>
                    )}

                    {ownerModule === 'schools' && (
                      <div className="owner-workspace">
                        <div className="table-list">
                          {dashboard.schools.map((school) => (
                            <button
                              className={`table-row owner-school-row ${schoolEditor.id === school.id ? 'owner-school-row-active' : ''}`}
                              key={school.id}
                              onClick={() => setSchoolEditor(mapSchoolToEditor(school))}
                              type="button"
                            >
                              <div>
                                <strong>{school.schoolName}</strong>
                                <p>{school.schoolCode} • {school.contactPersonName}</p>
                              </div>
                              <div>
                                <p>{school.studentCount} students</p>
                                <p>{school.facultyCount} faculty</p>
                              </div>
                              <span>{school.isActive ? 'Active' : 'Inactive'}</span>
                            </button>
                          ))}
                        </div>

                        {selectedSchool && (
                          <form className="panel owner-subpanel stack" onSubmit={saveSchoolManagement}>
                            <h4>School Details</h4>
                            <input value={schoolEditor.schoolName} disabled />
                            <input value={schoolEditor.schoolCode} disabled />
                            <input placeholder="Contact person" value={schoolEditor.contactPersonName} onChange={(e) => setSchoolEditor({ ...schoolEditor, contactPersonName: e.target.value })} />
                            <input placeholder="Email" value={schoolEditor.email} onChange={(e) => setSchoolEditor({ ...schoolEditor, email: e.target.value })} />
                            <input placeholder="Phone" value={schoolEditor.phone} onChange={(e) => setSchoolEditor({ ...schoolEditor, phone: e.target.value })} />
                            <input placeholder="Address" value={schoolEditor.address} onChange={(e) => setSchoolEditor({ ...schoolEditor, address: e.target.value })} />
                            <div className="hero-actions">
                              <button className="primary" disabled={loading}>Save School</button>
                              <button className="ghost" onClick={() => toggleSchoolStatus(selectedSchool)} type="button">
                                {selectedSchool.isActive ? 'Deactivate' : 'Activate'}
                              </button>
                            </div>
                          </form>
                        )}
                      </div>
                    )}

                    {ownerModule === 'billing' && selectedSchool && (
                      <div className="owner-workspace">
                        <form className="panel owner-subpanel stack" onSubmit={saveSchoolManagement}>
                          <h4>Billing & Licensing</h4>
                          <input placeholder="Plan" value={schoolEditor.subscriptionPlan} onChange={(e) => setSchoolEditor({ ...schoolEditor, subscriptionPlan: e.target.value })} />
                          <input placeholder="Billing cycle" value={schoolEditor.billingCycle} onChange={(e) => setSchoolEditor({ ...schoolEditor, billingCycle: e.target.value })} />
                          <input placeholder="Billing amount" type="number" value={schoolEditor.billingAmount} onChange={(e) => setSchoolEditor({ ...schoolEditor, billingAmount: e.target.value })} />
                          <input placeholder="Subscription start" type="date" value={schoolEditor.subscriptionStart} onChange={(e) => setSchoolEditor({ ...schoolEditor, subscriptionStart: e.target.value })} />
                          <input placeholder="Subscription end" type="date" value={schoolEditor.subscriptionEnd} onChange={(e) => setSchoolEditor({ ...schoolEditor, subscriptionEnd: e.target.value })} />
                          <input placeholder="Last billed on" type="date" value={schoolEditor.lastBillingDate} onChange={(e) => setSchoolEditor({ ...schoolEditor, lastBillingDate: e.target.value })} />
                          <input placeholder="Next billing date" type="date" value={schoolEditor.nextBillingDate} onChange={(e) => setSchoolEditor({ ...schoolEditor, nextBillingDate: e.target.value })} />
                          <input placeholder="License seats" type="number" value={schoolEditor.licenseSeats} onChange={(e) => setSchoolEditor({ ...schoolEditor, licenseSeats: e.target.value })} />
                          <input placeholder="Licensed modules" value={schoolEditor.licensedModules} onChange={(e) => setSchoolEditor({ ...schoolEditor, licensedModules: e.target.value })} />
                          <input placeholder="Billing notes" value={schoolEditor.billingNotes} onChange={(e) => setSchoolEditor({ ...schoolEditor, billingNotes: e.target.value })} />
                          <button className="primary" disabled={loading}>Save Billing</button>
                        </form>

                        <div className="panel owner-subpanel">
                          <h4>{selectedSchool.schoolName}</h4>
                          <div className="mini-list">
                            <div className="mini-item">Plan: {selectedSchool.subscriptionPlan}</div>
                            <div className="mini-item">Billing: {formatMoney(selectedSchool.billingAmount)} / {selectedSchool.billingCycle}</div>
                            <div className="mini-item">Seats: {selectedSchool.licenseSeats}</div>
                            <div className="mini-item">Modules: {selectedSchool.licensedModules || 'Core'}</div>
                            <div className="mini-item">Next bill: {formatDateValue(selectedSchool.nextBillingDate)}</div>
                            <div className="mini-item">Subscription end: {formatDateValue(selectedSchool.subscriptionEnd)}</div>
                          </div>
                        </div>
                      </div>
                    )}

                    {ownerModule === 'onboarding' && (
                      <div className="owner-workspace">
                        <article className="panel owner-subpanel">
                          <h4>Onboard School</h4>
                          <form className="stack compact" onSubmit={createOwnerSchool}>
                            {Object.entries(ownerSchoolForm).map(([key, value]) => (
                              <input
                                key={key}
                                placeholder={labelize(key)}
                                value={value}
                                onChange={(e) => setOwnerSchoolForm({ ...ownerSchoolForm, [key]: e.target.value })}
                                type={key.toLowerCase().includes('password') ? 'password' : 'text'}
                              />
                            ))}
                            <button className="primary" disabled={loading}>Create School</button>
                          </form>
                        </article>
                        <article className="panel owner-subpanel">
                          <h4>Onboarding Checklist</h4>
                          <div className="mini-list">
                            <div className="mini-item">Set the school contact and admin account.</div>
                            <div className="mini-item">Review billing plan and billing cycle.</div>
                            <div className="mini-item">Assign seats and modules before go-live.</div>
                            <div className="mini-item">Confirm activation once training is complete.</div>
                          </div>
                        </article>
                      </div>
                    )}
                  </article>
                </>
              )}

              {dashboard.role === 'SchoolAdmin' && (
                <>
                  <article className="panel">
                    <h3>Create Class</h3>
                    <form className="stack compact" onSubmit={createClass}>
                      {Object.entries(classForm).map(([key, value]) => (
                        <input
                          key={key}
                          placeholder={labelize(key)}
                          value={value}
                          onChange={(e) => setClassForm({ ...classForm, [key]: e.target.value })}
                        />
                      ))}
                      <button className="primary" disabled={loading}>Save Class</button>
                    </form>
                  </article>

                  <article className="panel">
                    <h3>Add Faculty</h3>
                    <form className="stack compact" onSubmit={createFaculty}>
                      {Object.entries(facultyForm).map(([key, value]) => (
                        <input
                          key={key}
                          placeholder={labelize(key)}
                          value={value}
                          onChange={(e) => setFacultyForm({ ...facultyForm, [key]: e.target.value })}
                          type={key === 'joiningDate' ? 'date' : key.toLowerCase().includes('password') ? 'password' : 'text'}
                        />
                      ))}
                      <button className="primary" disabled={loading}>Save Faculty</button>
                    </form>
                  </article>

                  <article className="panel">
                    <h3>Admit Student + Parent</h3>
                    <form className="stack compact" onSubmit={createStudent}>
                      {Object.entries(studentForm).map(([key, value]) => (
                        <input
                          key={key}
                          placeholder={labelize(key)}
                          value={value}
                          onChange={(e) => setStudentForm({ ...studentForm, [key]: e.target.value })}
                          type={key === 'dateOfBirth' ? 'date' : key.toLowerCase().includes('password') ? 'password' : 'text'}
                        />
                      ))}
                      <button className="primary" disabled={loading}>Save Student</button>
                    </form>
                  </article>

                  <article className="panel">
                    <h3>Holiday Calendar</h3>
                    <form className="stack compact" onSubmit={createHoliday}>
                      <input placeholder="Holiday name" value={holidayForm.name} onChange={(e) => setHolidayForm({ ...holidayForm, name: e.target.value })} />
                      <input type="date" value={holidayForm.date} onChange={(e) => setHolidayForm({ ...holidayForm, date: e.target.value })} />
                      <input placeholder="Description" value={holidayForm.description} onChange={(e) => setHolidayForm({ ...holidayForm, description: e.target.value })} />
                      <button className="primary" disabled={loading}>Add Holiday</button>
                    </form>
                    <div className="mini-list">
                      {holidays.map((holiday) => (
                        <div key={holiday.id} className="mini-item">
                          <strong>{holiday.name}</strong>
                          <span>{formatDate(holiday.date)}</span>
                        </div>
                      ))}
                    </div>
                  </article>

                  <article className="panel wide">
                    <h3>Classes, Faculty, Students</h3>
                    <div className="three-col">
                      <SimpleList title="Classes" items={dashboard.classes.map((item) => `${item.name} - ${item.section} • ${item.studentCount}`)} />
                      <SimpleList title="Faculty" items={dashboard.facultyMembers.map((item) => `${item.fullName} • ${item.department}`)} />
                      <SimpleList title="Students" items={dashboard.students.map((item) => `${item.fullName} • ${item.class} ${item.section}`)} />
                    </div>
                  </article>
                </>
              )}

              {dashboard.role === 'Faculty' && (
                <article className="panel wide">
                  <h3>Faculty View</h3>
                  <div className="three-col">
                    <SimpleList title="Assigned Classes" items={dashboard.classes.map((item) => `${item.name} - ${item.section}`)} />
                    <SimpleList title="Students" items={dashboard.students.map((item) => `${item.fullName} • Roll ${item.rollNumber}`)} />
                    <SimpleList title="Highlights" items={dashboard.highlights.map((item) => item.title)} />
                  </div>
                </article>
              )}

              {dashboard.role === 'Parent' && (
                <article className="panel wide">
                  <h3>Parent View</h3>
                  <div className="two-col">
                    <SimpleList title="Children" items={dashboard.children.map((child) => `${child.studentName} • ${child.className} ${child.section} • ${child.averageMarks}% marks`)} />
                    <SimpleList title="School Holidays" items={holidays.map((holiday) => `${holiday.name} • ${formatDate(holiday.date)}`)} />
                  </div>
                </article>
              )}

              {dashboard.role === 'Student' && (
                <article className="panel wide">
                  <h3>Student View</h3>
                  <div className="two-col">
                    <SimpleList title="My Profile" items={dashboard.students.map((student) => `${student.fullName} • ${student.class} ${student.section} • Roll ${student.rollNumber}`)} />
                    <SimpleList title="Upcoming Holidays" items={holidays.map((holiday) => `${holiday.name} • ${formatDate(holiday.date)}`)} />
                  </div>
                </article>
              )}
            </section>
          </>
        )}
      </main>
    </div>
  );
}

function SimpleList({ title, items }) {
  return (
    <div>
      <h4>{title}</h4>
      <div className="mini-list">
        {items.length === 0 ? <div className="mini-item muted">No items yet.</div> : items.map((item) => <div className="mini-item" key={item}>{item}</div>)}
      </div>
    </div>
  );
}

function labelize(value) {
  return value
    .replace(/([A-Z])/g, ' $1')
    .replace(/^./, (char) => char.toUpperCase());
}

function formatDate(value) {
  return new Date(value).toLocaleDateString(undefined, { day: '2-digit', month: 'short', year: 'numeric' });
}

export default App;
