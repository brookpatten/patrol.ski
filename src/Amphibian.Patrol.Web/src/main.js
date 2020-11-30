import 'core-js/stable'
import Vue from 'vue'
import CoreuiVue from '@coreui/vue'
import App from './App'
import router from './router/index'
import { iconsSet as icons } from './assets/icons/icons.js'
import store from './store'
import i18n from './i18n.js'

import VueAxios from 'vue-axios'
import Axios from 'axios'
import lodash from 'lodash'
import VueLodash from 'vue-lodash'
import VueBlockUI from 'vue-blockui'
import VueMoment from 'vue-moment'
import Moment from 'moment'

Vue.config.performance = true
Vue.use(CoreuiVue)
Vue.use(lodash,VueLodash)
Vue.use(VueBlockUI)
//Vue.use(VueMoment)
//Vue.use(Moment)
Vue.use(VueAxios, Axios)

const token = localStorage.getItem('token')
if (token) {
  Vue.prototype.$http.defaults.headers.common['Authorization'] = 'Bearer '+token
}

//globally configure axios so that any time an api call returns a 401, we redirect to login
Axios.interceptors.response.use(function (response) {
  return response
}, function (error) {
  console.log(error.response.data)
  if (error.response.data.status === 401) {
    console.log('401 - Logging out')
    store.dispatch('logout')
    router.push({name:'Login',params:{originalRoute:router.currentRoute}})
  }
  return Promise.reject(error)
})

new Vue({
  el: '#app',
  router,
  store,
  icons,
  template: '<App/>',
  components: {
    App
  },
  beforeCreate() { this.$store.commit('initializeState');}
})
