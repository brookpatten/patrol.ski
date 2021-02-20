import Vue from 'vue'
import Router from 'vue-router'
import store from '../store.js'

// Containers
const TheContainer = () => import('@/containers/TheContainer')
const PublicContainer = () => import('@/containers/PublicContainer')
const CenteredPublicContainer = () => import('@/containers/CenteredPublicContainer')

//error
const Page404 = () => import('@/views/pages/Page404')
const Page500 = () => import('@/views/pages/Page500')

//login
const Login = () => import('@/views/pages/Login')
const Register = () => import('@/views/pages/Register')

//public pages
const Landing = () => import('@/views/pages/Landing')
const Help = () => import('@/views/pages/Help')
const TestDrive = () => import('@/views/pages/TestDrive')
const Privacy = () => import('@/views/pages/Privacy')
const Delete = () => import('@/views/pages/Delete')
const Terms = () => import('@/views/pages/Terms')
const Subdomain = () => import('@/views/pages/Subdomain')

// App
const Plan = () => import('@/views/schedule/Plan')
const Home = () => import('@/views/schedule/Home')
const Assignment = () => import('@/views/schedule/Assignment')
const MyCalendar = () => import('@/views/schedule/Calendar')
const ScheduleSwap = () => import('@/views/schedule/ScheduleSwap')
const Profile = () => import('@/views/schedule/Profile')

// Administration
const Administration = () => import('@/views/administration/Administration')
const Groups = () => import('@/views/administration/Groups')
const People = () => import('@/views/administration/People')
const Plans = () => import('@/views/administration/Plans')
const EditPlan = () => import('@/views/administration/EditPlan')
const EditUser = () => import('@/views/administration/EditUser')
const EditGroup = () => import('@/views/administration/EditGroup')
const EditAnnouncement = () => import('@/views/administration/EditAnnouncement')
const Assignments = () => import('@/views/administration/Assignments')
const NewAssignment = () => import('@/views/administration/NewAssignment')
const EditAssignment = () => import('@/views/administration/EditAssignment')
const NewPatrol = () => import('@/views/administration/NewPatrol')
const Announcements = () => import('@/views/administration/Announcements')
const EditPatrol = () => import('@/views/administration/EditPatrol')
const Events = () => import('@/views/administration/Events')
const EditEvent = () => import('@/views/administration/EditEvent')
const Shifts = () => import('@/views/administration/Shifts')
const EditShift = () => import('@/views/administration/EditShift')
const SwapApproval = () => import('@/views/administration/SwapApproval')
const TimeEntries = () => import('@/views/administration/TimeEntries')
const TimeDays = () => import('@/views/administration/TimeDays')
const TimeMissing = () => import('@/views/administration/TimeMissing')
const RepeatSchedule = () => import('@/views/administration/RepeatSchedule')
const WorkItems = () => import('@/views/administration/WorkItems')
const RecurringWorkItems = () => import('@/views/administration/RecurringWorkItems')

//sysadmin
const SysAdminDashboard = () => import('@/views/sysadmin/Dashboard')

Vue.use(Router)

let router = new Router({
  mode: 'hash', // https://router.vuejs.org/api/#mode
  linkActiveClass: 'active',
  scrollBehavior: () => ({ y: 0 }),
  routes: configRoutes()
})

router.beforeEach((to, from, next) => {
  var host = window.location.host;
  if(host.indexOf(':')>=0){
    var portParts = host.split(':');
    host = portParts[0];
    var port = portParts[1];
  }
  var hostParts = host.split('.');

  if(to.matched.some(record => record.meta.requiresAuth)) {
    //if the page being requested needs auth, redirect to auth if it's missing
    console.log('checking auth');
    if (store.getters.isLoggedIn) {
      next()
      return
    }
    console.log('not logged in');
    next('/login') 
  }
  else if(to.matched.some(record=> record.name=='Landing') && store.getters.isLoggedIn){
    //if the user ended up on the landing page (bookmark etc) and they are already logged in, move them to the app
    next('/app') 
  }
  else if(to.matched.some(record=> record.name=='Landing') 
    && hostParts.length>2
    ){
    var subdomain = "";
    for(var i=0;i<hostParts.length-2;i++){
      if(subdomain.length>0){
        subdomain = subdomain+".";
      }
      subdomain = subdomain+hostParts[i];
    }
    if(subdomain!="www"){
      next({name:'Subdomain',params:{subdomain:subdomain}}); 
    }
    else
    {
      return next();
    }
  }
  else if(to.matched.some(record=> record.name=='Landing') 
    && hostParts.length>1 
    && hostParts[hostParts.length-1] == "localhost"
    ){
    var subdomain = "";
    for(var i=0;i<hostParts.length-1;i++){
      if(subdomain.length>0){
        subdomain = subdomain+".";
      }
      subdomain = subdomain+hostParts[i];
    }
    if(subdomain!="www"){
      next({name:'Subdomain',params:{subdomain:subdomain}}); 
    }
    else
    {
      return next();
    }
  } 
  else {
    next() 
  }
  
})

export default router;

function configRoutes () {
  return [
    {
      path: '/app',
      name: '',
      component: TheContainer,
      children: [
        {
          path: '',
          name: 'Home',
          component: Home,
          meta: { 
            requiresAuth: true
          }
        },
        {
          path: 'plan/:planId',
          name: 'Plan',
          component: Plan,
          meta: { 
            requiresAuth: true
          },
          props: true
        },
        {
          path: 'assignment/:assignmentId',
          name: 'Assignment',
          component: Assignment,
          meta: { 
            requiresAuth: true
          },
          props: true
        },
        {
          path: 'calendar',
          name: 'Calendar',
          component: MyCalendar,
          meta: { 
            requiresAuth: true
          },
          props: true
        },
        {
          path: 'profile',
          name: 'Profile',
          component: Profile,
          meta: { 
            requiresAuth: true
          }
        },
        {
          path: 'schedule-swap',
          name: 'ScheduleSwap',
          component: ScheduleSwap,
          meta: {
            requiresAuth: true
          }
        },
        {
          path: 'sysadmin',
          component: {
            render (c) {return c('router-view') }
          },
          children: [
            {
              path: '',
              name: 'SysAdmin Dashboard',
              component: SysAdminDashboard,
              meta: { 
                requiresAuth: true
              }
            }
          ]
        },
        {
          path: 'admin',
          //redirect: '/administration/Administration',
          //name: 'Administration',
          component: {
            render (c) { return c('router-view') }
          },
          children: [
            {
              path: '',
              name: 'Administration',
              component: Administration,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'people',
              name: 'People',
              component: People,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'groups',
              name: 'Groups',
              component: Groups,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'announcements',
              name: 'Announcements',
              component: Announcements,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'plans',
              name: 'Plans',
              component: Plans,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'plan/:planId',
              name: 'EditPlan',
              component: EditPlan,
              meta: { 
                requiresAuth: true
              },
              props:true
            },
            {
              path: 'edit-patrol',
              name: 'EditPatrol',
              component: EditPatrol,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'user/:userId',
              name: 'EditUser',
              component: EditUser,
              meta: {
                requiresAuth: true
              },
              props: true
            },
            {
              path: 'group/:groupId',
              name: 'EditGroup',
              component: EditGroup,
              meta: {
                requiresAuth: true
              },
              props: true
            },
            {
              path: 'announcement/:announcementId',
              name: 'EditAnnouncement',
              component: EditAnnouncement,
              meta: {
                requiresAuth: true
              },
              props: true
            },
            {
              path: 'assignments',
              name: 'Assignments',
              component: Assignments,
              meta: { 
                requiresAuth: true
              },
              props: true
            },
            {
              path: 'events',
              name: 'Events',
              component: Events,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'new-assignment/:planId',
              name: 'NewAssignment',
              component: NewAssignment,
              meta: { 
                requiresAuth: true
              },
              props: true
            },
            {
              path: 'edit-assignment/:assignmentId',
              name: 'EditAssignment',
              component: EditAssignment,
              meta: { 
                requiresAuth: true
              },
              props: true
            },
            {
              path: 'edit-event/:eventId',
              name: 'EditEvent',
              component: EditEvent,
              meta: { 
                requiresAuth: true
              },
              props: true
            },
            {
              path: 'new-patrol',
              name: 'NewPatrol',
              component: NewPatrol,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'shifts',
              name: 'Shifts',
              component: Shifts,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'edit-shift/:shiftId',
              name: 'EditShift',
              component: EditShift,
              meta: { 
                requiresAuth: true
              },
              props: true
            },
            {
              path: 'swap-approval',
              name: 'SwapApproval',
              component: SwapApproval,
              meta: {
                requiresAuth: true
              }
            },
            {
              path: 'time-entries',
              name: 'TimeEntries',
              component: TimeEntries,
              meta: {
                requiresAuth: true
              },
              props:true
            },
            {
              path: 'time-days',
              name: 'TimeDays',
              component: TimeDays,
              meta: {
                requiresAuth: true
              },
              props:true
            },
            {
              path: 'time-missing',
              name: 'TimeMissing',
              component: TimeMissing,
              meta: {
                requiresAuth: true
              },
              props:true
            },
            {
              path: 'repeat-schedule',
              name: 'RepeatSchedule',
              component: RepeatSchedule,
              meta: {
                requiresAuth: true
              }
            },
            {
              path: 'work-items',
              name: 'WorkItems',
              component: WorkItems,
              meta: {
                requiresAuth: true
              }
            },
            {
              path: 'recurring-work-items',
              name: 'RecurringWorkItems',
              component: RecurringWorkItems,
              meta: {
                requiresAuth: true
              }
            }
          ]
        }
      ]
    },
    {
      path: '/login',
      component: CenteredPublicContainer,
      children: [
        {
          path: '',
          name: 'Login',
          component: Login
        },
        {
          path: 'register',
          name: 'Register',
          component: Register
        }
      ]
    },
    {
      path: '/error',
      name: 'Error',
      component: CenteredPublicContainer,
      children: [
        {
          path: '404',
          name: 'Page404',
          component: Page404
        },
        {
          path: '500',
          name: 'Page500',
          component: Page500
        }
      ]
    },
    {
      path: '/',
      name: '',
      component: PublicContainer,
      children: [
        {
          path: '',
          name: 'Landing',
          component: Landing
        },
        {
          path: 'public',
          name: 'Subdomain',
          component: Subdomain,
          props: true
        },
        {
          path: 'help',
          name: 'Help',
          component: Help
        },
        {
          path: 'test-drive',
          name: 'TestDrive',
          component: TestDrive
        },
        {
          path: 'privacy',
          name: 'Privacy',
          component: Privacy
        },
        {
          path: 'delete',
          name: 'Delete',
          component: Delete
        },
        {
          path: 'terms',
          name: 'Terms',
          component: Terms
        }
      ]
    }
  ]
}

