import 'core-js/stable'
import Vue from 'vue'
import App from './App'
import router from './router'
import CoreuiVue from '@coreui/vue'
import { iconsSet as icons } from './assets/icons/icons.js'
import store from './store'

import VueAxios from 'vue-axios'
import Axios from 'axios'
import lodash from 'lodash'
import VueLodash from 'vue-lodash'
import VueBlockUI from 'vue-blockui'

Vue.config.performance = true
Vue.use(CoreuiVue)
Vue.use(VueLodash, lodash)
Vue.use(VueBlockUI)
Vue.use(VueAxios, Axios)

const token = localStorage.getItem('token')
if (token) {
  Vue.prototype.$http.defaults.headers.common['Authorization'] = 'Token '+token
}

new Vue({
  el: '#app',
  router,
  store,
  icons,
  template: '<App/>',
  components: {
    App
  }
})
