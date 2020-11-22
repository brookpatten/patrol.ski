import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

Vue.use(Vuex)

const state = {
  sidebarShow: 'responsive',
  sidebarMinimize: false,
  status: '',
  token: localStorage.getItem('token') || '',
  user: {},
  patrols: [],
  selectedPatrolId: 0
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
  auth_success(state, data){
    state.status = 'success';
    state.token = data.token;
    state.user = data.user;
    state.patrols = data.patrols;
    if(data.patrols && data.patrols.length>0){
      state.selectedPatrolId = data.patrols[0].id;
    }
  },
  auth_error(state){
    state.status = 'error';
  },
  logout(state){
    state.status = '';
    state.token = '';
  },
  change_patrol(state,data){
    state.selectedPatrolId = data;
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
          localStorage.setItem('token', token);
          axios.defaults.headers.common['Authorization'] = 'Token ' + token;
          commit('auth_success', resp.data);
          resolve(resp);
        })
        .catch(err => {
          commit('auth_error');
          localStorage.removeItem('token');
          reject(err);
        });
      });
    },
    change_patrol({commit}, patrolId){
      return new Promise((resolve, reject) => {
        commit('change_patrol', patrolId);
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
          localStorage.setItem('token', token);
          axios.defaults.headers.common['Authorization'] = 'Token ' + token;
          commit('auth_success', resp.data);
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
    authStatus: state => state.status
  }
})