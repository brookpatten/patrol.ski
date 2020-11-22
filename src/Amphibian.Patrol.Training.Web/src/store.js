import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

Vue.use(Vuex)

const state = {
  sidebarShow: 'responsive',
  sidebarMinimize: false,
  status: '',
  token: localStorage.getItem('token') || '',
  user: {}
}

const mutations = {
  toggleSidebarDesktop (state) {
    const sidebarOpened = [true, 'responsive'].includes(state.sidebarShow)
    state.sidebarShow = sidebarOpened ? false : 'responsive'
  },
  toggleSidebarMobile (state) {
    const sidebarClosed = [false, 'responsive'].includes(state.sidebarShow)
    state.sidebarShow = sidebarClosed ? true : 'responsive'
  },
  set (state, [variable, value]) {
    state[variable] = value;
  },
  auth_request(state){
    state.status = 'loading';
  },
  auth_success(state, token, user){
    state.status = 'success';
    state.token = token;
    state.user = user;
  },
  auth_error(state){
    state.status = 'error';
  },
  logout(state){
    state.status = '';
    state.token = '';
  }
}

export default new Vuex.Store({
  state,
  mutations,
  actions: {
    login({commit}, user){
      return new Promise((resolve, reject) => {
        commit('auth_request');
        axios.post('user/authenticate',user)
        .then(resp => {
          const token = resp.data.token;
          const user = resp.data;
          localStorage.setItem('token', token);
          axios.defaults.headers.common['Authorization'] = 'Token ' + token;
          commit('auth_success', token, user);
          resolve(resp);
        })
        .catch(err => {
          commit('auth_error');
          localStorage.removeItem('token');
          reject(err);
        });
      });
    },
    register({commit}, user){
      console.log("store register");
      return new Promise((resolve, reject) => {
        commit('auth_request');
        console.log(user);
        axios.post('user/register',user)
        .then(resp => {
          console.log('auth success',resp);
          const token = resp.data.token;
          const user = resp.data;
          localStorage.setItem('token', token);
          axios.defaults.headers.common['Authorization'] = 'Token ' + token;
          commit('auth_success', token, user);
          resolve(resp);
        })
        .catch(err => {
          console.log(err);
          commit('auth_error', err);
          localStorage.removeItem('token');
          reject(err);
        });
      });
    },
    logout({commit}){
      return new Promise((resolve, reject) => {
        commit('logout');
        localStorage.removeItem('token');
        delete axios.defaults.headers.common['Authorization'];
        resolve();
      });
    }
  },
  getters: {
    isLoggedIn: state => !!state.token,
    authStatus: state => state.status,
  }
})