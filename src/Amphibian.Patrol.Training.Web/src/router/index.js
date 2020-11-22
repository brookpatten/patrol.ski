import Vue from 'vue'
import Router from 'vue-router'
import store from '../store.js'

// Containers
const TheContainer = () => import('@/containers/TheContainer')
const PublicContainer = () => import('@/containers/PublicContainer')

// Views
const Dashboard = () => import('@/views/Dashboard')

const Colors = () => import('@/views/theme/Colors')
const Typography = () => import('@/views/theme/Typography')

const Charts = () => import('@/views/charts/Charts')
const Widgets = () => import('@/views/widgets/Widgets')

// Views - Components
const Cards = () => import('@/views/base/Cards')
const Forms = () => import('@/views/forms/Forms')
const Switches = () => import('@/views/base/Switches')
const Tables = () => import('@/views/tables/Tables')
const Tabs = () => import('@/views/base/Tabs')
const Breadcrumbs = () => import('@/views/base/Breadcrumbs')
const Carousels = () => import('@/views/base/Carousels')
const Collapses = () => import('@/views/base/Collapses')
const Jumbotrons = () => import('@/views/base/Jumbotrons')
const ListGroups = () => import('@/views/base/ListGroups')
const Navs = () => import('@/views/base/Navs')
const Navbars = () => import('@/views/base/Navbars')
const Paginations = () => import('@/views/base/Paginations')
const Popovers = () => import('@/views/base/Popovers')
const ProgressBars = () => import('@/views/base/ProgressBars')
const Tooltips = () => import('@/views/base/Tooltips')

// Views - Buttons
const StandardButtons = () => import('@/views/buttons/StandardButtons')
const ButtonGroups = () => import('@/views/buttons/ButtonGroups')
const Dropdowns = () => import('@/views/buttons/Dropdowns')
const BrandButtons = () => import('@/views/buttons/BrandButtons')

// Views - Icons
const CoreUIIcons = () => import('@/views/icons/CoreUIIcons')
const Brands = () => import('@/views/icons/Brands')
const Flags = () => import('@/views/icons/Flags')

// Views - Notifications
const Alerts = () => import('@/views/notifications/Alerts')
const Badges = () => import('@/views/notifications/Badges')
const Modals = () => import('@/views/notifications/Modals')

// Views - Pages
const Page404 = () => import('@/views/pages/Page404')
const Page500 = () => import('@/views/pages/Page500')
const Login = () => import('@/views/pages/Login')
const Register = () => import('@/views/pages/Register')
const Landing = () => import('@/views/pages/Landing')

// Users
const Users = () => import('@/views/users/Users')
const User = () => import('@/views/users/User')

// Schedule
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
    next('/pages/login') 
  } else {
    next() 
  }
})

export default router;

function configRoutes () {
  return [
    {
      path: '/',
      redirect: '/home',
      name: '',
      component: TheContainer,
      children: [
        {
          path: 'home',
          name: 'Home',
          component: Home,
          meta: { 
            requiresAuth: true
          }
        },
        {
          path: 'dashboard',
          name: 'Dashboard',
          component: Dashboard,
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
          path: 'administration',
          redirect: '/administration/Administration',
          //name: 'Administration',
          component: {
            render (c) { return c('router-view') }
          },
          children: [
            {
              path: 'administration',
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
            }
          ]
        },
        {
          path: 'theme',
          redirect: '/theme/colors',
          name: 'Theme',
          component: {
            render (c) { return c('router-view') }
          },
          children: [
            {
              path: 'colors',
              name: 'Colors',
              component: Colors
            },
            {
              path: 'typography',
              name: 'Typography',
              component: Typography
            }
          ]
        },
        {
          path: 'charts',
          name: 'Charts',
          component: Charts
        },
        {
          path: 'widgets',
          name: 'Widgets',
          component: Widgets
        },
        {
          path: 'users',
          meta: {
            label: 'Users'
          },
          component: {
            render(c) {
              return c('router-view')
            }
          },
          children: [
            {
              path: '',
              name: 'Users',
              component: Users
            },
            {
              path: ':id',
              meta: {
                label: 'User Details'
              },
              name: 'User',
              component: User
            }
          ]
        },
        {
          path: 'base',
          redirect: '/base/cards',
          name: 'Base',
          component: {
            render (c) { return c('router-view') }
          },
          children: [
            {
              path: 'cards',
              name: 'Cards',
              component: Cards
            },
            {
              path: 'forms',
              name: 'Forms',
              component: Forms
            },
            {
              path: 'switches',
              name: 'Switches',
              component: Switches
            },
            {
              path: 'tables',
              name: 'Tables',
              component: Tables
            },
            {
              path: 'tabs',
              name: 'Tabs',
              component: Tabs
            },
            {
              path: 'breadcrumbs',
              name: 'Breadcrumbs',
              component: Breadcrumbs
            },
            {
              path: 'carousels',
              name: 'Carousels',
              component: Carousels
            },
            {
              path: 'collapses',
              name: 'Collapses',
              component: Collapses
            },
            {
              path: 'jumbotrons',
              name: 'Jumbotrons',
              component: Jumbotrons
            },
            {
              path: 'list-groups',
              name: 'List Groups',
              component: ListGroups
            },
            {
              path: 'navs',
              name: 'Navs',
              component: Navs
            },
            {
              path: 'navbars',
              name: 'Navbars',
              component: Navbars
            },
            {
              path: 'paginations',
              name: 'Paginations',
              component: Paginations
            },
            {
              path: 'popovers',
              name: 'Popovers',
              component: Popovers
            },
            {
              path: 'progress-bars',
              name: 'Progress Bars',
              component: ProgressBars
            },
            {
              path: 'tooltips',
              name: 'Tooltips',
              component: Tooltips
            }
          ]
        },
        {
          path: 'buttons',
          redirect: '/buttons/standard-buttons',
          name: 'Buttons',
          component: {
            render (c) { return c('router-view') }
          },
          children: [
            {
              path: 'standard-buttons',
              name: 'Standard Buttons',
              component: StandardButtons
            },
            {
              path: 'button-groups',
              name: 'Button Groups',
              component: ButtonGroups
            },
            {
              path: 'dropdowns',
              name: 'Dropdowns',
              component: Dropdowns
            },
            {
              path: 'brand-buttons',
              name: 'Brand Buttons',
              component: BrandButtons
            }
          ]
        },
        {
          path: 'icons',
          redirect: '/icons/coreui-icons',
          name: 'CoreUI Icons',
          component: {
            render (c) { return c('router-view') }
          },
          children: [
            {
              path: 'coreui-icons',
              name: 'Icons library',
              component: CoreUIIcons
            },
            {
              path: 'brands',
              name: 'Brands',
              component: Brands
            },
            {
              path: 'flags',
              name: 'Flags',
              component: Flags
            }
          ]
        },
        {
          path: 'notifications',
          redirect: '/notifications/alerts',
          name: 'Notifications',
          component: {
            render (c) { return c('router-view') }
          },
          children: [
            {
              path: 'alerts',
              name: 'Alerts',
              component: Alerts
            },
            {
              path: 'badges',
              name: 'Badges',
              component: Badges
            },
            {
              path: 'modals',
              name: 'Modals',
              component: Modals
            }
          ]
        }
      ]
    },
    {
      path: '/pages',
      redirect: '/pages/404',
      name: 'Pages',
      component: PublicContainer,
      children: [
        {
          path: 'landing',
          name: 'Landing',
          component: Landing
        },
        {
          path: '404',
          name: 'Page404',
          component: Page404
        },
        {
          path: '500',
          name: 'Page500',
          component: Page500
        },
        {
          path: 'login',
          name: 'Login',
          component: Login
        },
        {
          path: 'register',
          name: 'Register',
          component: Register
        }
      ]
    }
  ]
}

