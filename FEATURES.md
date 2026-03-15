# eSchool Feature List

This file is the working feature checklist for the multi-tenant eSchool app.

You can:
- add missing features
- mark items as approved
- edit wording
- add priorities like `P1`, `P2`
- add notes under any section

## Product Vision
- Multi-tenant school management app
- Mobile app plus simple supporting web app
- The same app should be accessible in two ways: mobile app and web app
- One app owner can manage the whole platform
- Multiple schools can be onboarded under the same app
- Each school has its own admins, faculty, parents, and students
- Data stays isolated per school
- UI should feel catchy, friendly, modern, and clearly school-focused
- UI should be simple enough for daily use by non-technical school staff

## UI and Experience Principles
- Catchy and school-friendly visual design
- Bright, welcoming, and trustworthy look
- Easy navigation for admins, teachers, parents, and students
- Dashboard-first experience for each role
- Minimal clutter and quick actions for daily school tasks
- Mobile-friendly layouts
- Clear icons, cards, and section-based navigation
- Forms should be easier than prompt dialogs in the final version
- Important school actions should be reachable in 1 to 3 taps
- Use school-oriented wording instead of technical wording

## Web App Requirements
- Simple landing page
- Login-first web experience
- Clean and minimal school-friendly design
- After login, web should expose the same implemented role-based features available in mobile app
- Web and mobile should act as two access channels to the same product
- Contact support section for new school onboarding
- Show onboarding help details on the login page
- Show app owner/support email
- Show support phone number
- Show WhatsApp or helpdesk contact if available
- Quick message like `Need your school onboarded? Contact support`
- Web page should clearly guide schools on how to get access
- Responsive layout for desktop and mobile browser

## App Details
- App name: eSchool
- Multi-tenant school ERP and communication platform
- Access modes: mobile app and web app
- Supports app owner, school admin, faculty, parent, and student roles
- School users log in using school code and username
- App owner logs in using email
- Each school has separate data and role-based access
- School onboarding can be managed centrally
- School admin runs all school-level operations
- Designed for daily school workflows like admissions, classes, attendance, marks, and communication

## Login and Onboarding Details
- Mobile app login page
- Web app login page
- Owner login using email and password
- School user login using `SCHOOLCODE@username`
- Support contact details visible on onboarding/login surfaces
- New schools should know exactly whom to contact for onboarding
- No self-service school signup
- School signup is created only by app owner/admin after pricing and billing discussion
- Optional request-demo or contact-support flow on web app
- FAQ/help text for login format
- Friendly error messages for wrong login format or inactive school

## Roles
- App Owner
- School Admin
- Faculty
- Parent
- Student

## Core Platform Features
- Multi-tenant school onboarding
- Tenant isolation by school
- Role-based authentication
- Role-based dashboard
- Feature parity between implemented mobile and web role dashboards
- School code based login for tenant users
- Owner email login
- Active/inactive school control
- School subscription start/end tracking
- School profile management
- Basic audit-friendly created dates on records
- Contact support information for onboarding

## App Owner Features
- View all onboarded schools
- Create a new school from the app
- Generate/manage school code
- Create school admin during onboarding
- Activate/deactivate a school
- View total schools, students, faculty, and classes across the platform
- View school-wise stats
- Manage platform-level configuration
- Reset or manage school admin access
- View platform usage summary
- Manage support and onboarding contact details shown in app/web

## School Admin Features
- School dashboard
- Manage school profile
- Full control over all school-level data and settings
- Create classes
- Create sections
- Set class capacity
- Assign class teacher
- Manage subjects
- Manage timetable
- Manage exams
- Manage marks
- Manage attendance
- Manage holidays
- Manage notices and announcements
- Manage events
- Add faculty
- Edit faculty
- Remove faculty
- Create student account
- Create parent account while adding student
- Link one parent to multiple children
- Edit student profile
- Remove student
- Manage parent accounts
- Reset passwords for faculty, parents, and students
- Activate/deactivate user accounts inside the school
- Manage roll numbers
- Search/filter students
- Search/filter faculty
- Search/filter parents
- View all classes, faculty, students, and parents from one place
- Manage school operational modules like fees, transport, library, and support when enabled
- View school-wide summaries

## Student Management Features
- Student admission
- Student profile
- Student login
- Student class and section assignment
- Parent details capture
- Date of birth capture
- Unique username per school
- Student directory
- Transfer student between classes/sections
- Student status management
- Student ID/admission number
- Medical info
- Address info
- Emergency contact

## Parent Features
- Parent login
- Parent dashboard
- View linked children
- View attendance summary
- View marks summary
- View child class/section
- Single parent account for siblings
- View school notices
- View holiday calendar
- View fee summary
- View homework/assignments
- Teacher communication inbox

## Faculty Features
- Faculty login
- Faculty dashboard
- View assigned classes
- View student directory
- View attendance data
- Add attendance
- Update attendance
- Add marks
- Update marks
- View exam performance
- Class teacher visibility
- View timetable
- View notices from admin
- Manage homework/assignments
- Parent communication

## Student Features
- Student login
- Student dashboard
- View profile
- View attendance
- View marks/results
- View class timetable
- View homework/assignments
- View notices
- View holidays
- View exam schedule

## Academic Features
- Class management
- Section management
- Subject management
- Exam management
- Marks entry
- Grade calculation
- Report cards
- Attendance tracking
- Leave management
- Homework management
- Timetable management
- Promotion to next class

## Communication Features
- Announcements
- School notices
- Parent notifications
- Faculty notices
- Student notices
- In-app alerts
- Push notifications
- Message center
- Event reminders
- Support contact visibility on web and app onboarding surfaces

## Operations Features
- Holiday calendar
- Event calendar
- Fee management
- Fee reminders
- Transport management
- Hostel management
- Library management
- Inventory/basic asset tracking
- Complaint/support tickets

## Security and Access
- JWT/token-based auth
- Role-based API authorization
- School-level data isolation
- Password hashing
- Disable inactive schools
- Session management
- Access control by module
- Reset password flow
- Change password flow

## Reports and Analytics
- Student strength by class
- Faculty count
- Attendance reports
- Marks performance reports
- Parent engagement summary
- School growth summary
- Platform-wide tenant analytics

## Nice-to-Have Features
- Custom branding per school
- White-label support
- Multi-language support
- Dark mode
- Offline sync for attendance
- Document upload
- ID card generation
- QR-based attendance
- Biometric integration
- WhatsApp/SMS integration

## Suggested Next Additions
- Proper edit screens instead of prompt dialogs
- Full CRUD pages for classes and faculty
- Full school admin control center for all school modules
- Simple web login page with onboarding support/contact section
- Subject and timetable modules
- Parent/student dedicated pages beyond dashboard summary
- Fee management module
- Notification module
- Audit log module

## Change Notes
- Add your comments here
- Example: `Need separate accountant role`
- Example: `Parent should be able to pay fees from app`
