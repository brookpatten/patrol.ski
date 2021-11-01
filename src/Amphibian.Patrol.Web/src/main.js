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
import VueLazyLoad from 'vue-lazyload'
import VueCookies from 'vue-cookies'

Vue.config.performance = true
Vue.use(CoreuiVue)
Vue.use(lodash,VueLodash)
Vue.use(VueBlockUI)
Vue.use(VueLazyLoad)
Vue.use(VueAxios, Axios)
Vue.use(VueCookies)

//const token = localStorage.getItem('token')
//const token = Vue.$cookies.get('access_token');
//if (token) {
//  Vue.prototype.$http.defaults.headers.common['Authorization'] = 'Bearer '+token
//}

//globally configure axios so that any time an api call returns a 401, we redirect to login
Axios.interceptors.response.use(function (response) {
  //check for updated token
  if(response.headers.authorization){
    var jwt = response.headers.authorization.split(' ')[1];
    if(jwt!=store.getters.token){
      store.dispatch('authenticate',jwt);
    }
  }
return response
}, function (error) {
  console.log(error.response.data)
  
  //check for updated token
  if(error.response.headers.Authorization){
    var jwt = error.response.headers.Authorization.split(' ')[1];
    if(jwt!=store.getters.token){
      store.dispatch('authenticate',jwt);
    }
  }

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
  beforeCreate() { }
})
