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

// App
const Plan = () => import('@/views/schedule/Plan')
const Home = () => import('@/views/schedule/Home')
const Assignment = () => import('@/views/schedule/Assignment')

// Administration
const Administration = () => import('@/views/administration/Administration')
const Groups = () => import('@/views/administration/Groups')
const Integration = () => import('@/views/administration/Integration')
const People = () => import('@/views/administration/People')
const Plans = () => import('@/views/administration/Plans')
const EditPlan = () => import('@/views/administration/EditPlan')
const Skills = () => import('@/views/administration/Skills')
const Levels = () => import('@/views/administration/Levels')
const EditUser = () => import('@/views/administration/EditUser')
const EditGroup = () => import('@/views/administration/EditGroup')
const Assignments = () => import('@/views/administration/Assignments')
const NewAssignment = () => import('@/views/administration/NewAssignment')
const EditAssignment = () => import('@/views/administration/EditAssignment')
const NewPatrol = () => import('@/views/administration/NewPatrol')

Vue.use(Router)

let router = new Router({
  mode: 'hash', // https://router.vuejs.org/api/#mode
  linkActiveClass: 'active',
  scrollBehavior: () => ({ y: 0 }),
  routes: configRoutes()
})

router.beforeEach((to, from, next) => {
  if(to.matched.some(record => record.meta.requiresAuth)) {
    if (store.getters.isLoggedIn) {
      next()
      return
    }
    next('/login') 
  } else {
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
              path: 'integration',
              name: 'Integration',
              component: Integration,
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
              path: 'skills',
              name: 'Skills',
              component: Skills,
              meta: { 
                requiresAuth: true
              }
            },
            {
              path: 'levels',
              name: 'Levels',
              component: Levels,
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
              path: 'assignments',
              name: 'Assignments',
              component: Assignments,
              meta: { 
                requiresAuth: true
              },
              props: true
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
              path: 'new-patrol',
              name: 'NewPatrol',
              component: NewPatrol,
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
      name: 'Login',
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
          path: 'help',
          name: 'Help',
          component: Help
        },
        {
          path: 'test-drive',
          name: 'TestDrive',
          component: TestDrive
        },
      ]
    }
  ]
}

