import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import lodash from 'lodash'
import VueLodash from 'vue-lodash'

Vue.use(Vuex)
Vue.use(VueLodash, lodash)

const state = {
  sidebarShow: 'responsive',
  sidebarMinimize: false,
  status: '',
  token: localStorage.getItem('token') || '',
  user: localStorage.getItem('user') != null ? JSON.parse(localStorage.getItem('user')) :null,
  patrols:  localStorage.getItem('patrols')!=null ? (JSON.parse(localStorage.getItem('patrols')).patrols) :[],
  selectedPatrolId: localStorage.getItem('selectedPatrolId') !=null ? parseInt(localStorage.getItem('selectedPatrolId')) : 0,
  loadingMessage: 'Loading',
  loadingCount: 0
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
  },
  auth_error(state){
    state.status = 'error';
  },
  logout(state){
    state.status = '';
    state.token = null;
  },
  change_patrol(state,data){
    state.selectedPatrolId = data;
  },
  update_patrols(state,data){
    state.patrols = data.patrols;
    state.selectedPatrolId = data.id;
  },
  toggle (state, variable) {
    state[variable] = !state[variable]
  },
  loading (state,message){
    state.loadingMessage = message;
    state.loadingCount++;
  },
  loadingComplete(state,message){
    state.loadingCount--;
    if(state.loadingCount<0){
      state.loadingCount=0;
    }
  }
}

export default new Vuex.Store({
  state,
  mutations,
  actions: {
    update_patrols({commit},patrols){
      return new Promise((resolve, reject) => {
        localStorage.setItem('patrols', {patrols:patrols.patrol});
        commit('update_patrols', patrols);
        resolve();
      });
    },
    login({commit}, user){
      return new Promise((resolve, reject) => {
        commit('auth_request');
        axios.post('user/authenticate'+(user.throwaway ? '-throwaway' : ''),user)
        .then(resp => {
          commit('auth_success', resp.data);
          localStorage.setItem('user', JSON.stringify(state.user));
          localStorage.setItem('token', state.token);
          axios.defaults.headers.common['Authorization'] = 'Bearer ' + state.token;
          localStorage.setItem('patrols', JSON.stringify({patrols:state.patrols}));
          if(state.patrols && state.patrols.length>0){
            state.selectedPatrolId = state.patrols[0].id;
            localStorage.setItem('selectedPatrolId', state.selectedPatrolId);
          }
          else {
            //redirect the user to create a patrol
          }

          resolve(resp);
        })
        .catch(err => {
          commit('auth_error');
          localStorage.removeItem('user');
          localStorage.removeItem('token');
          localStorage.removeItem('patrols');
          localStorage.removeItem('selectedPatrolId');
          reject(err);
        });
      });
    },
    change_patrol({commit}, patrolId){
      return new Promise((resolve, reject) => {
        localStorage.setItem('selectedPatrolId', patrolId);
        commit('change_patrol', patrolId);
        resolve();
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
          commit('auth_success', resp.data);
          localStorage.setItem('user', JSON.stringify(state.user));
          localStorage.setItem('token', state.token);
          axios.defaults.headers.common['Authorization'] = 'Bearer ' + state.token;
          localStorage.setItem('patrols', JSON.stringify({patrols:state.patrols}));
          if(state.patrols && state.patrols.length>0){
            state.selectedPatrolId = state.patrols[0].id;
            localStorage.setItem('selectedPatrolId', state.selectedPatrolId);
          }

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
        localStorage.removeItem('user');
        localStorage.removeItem('token');
        localStorage.removeItem('patrols');
        localStorage.removeItem('selectedPatrolId');
        delete axios.defaults.headers.common['Authorization'];
        resolve();
      });
    },
    loading({commit},message){
      commit('loading',message);
    },
    loadingComplete({commit}){
      commit('loadingComplete');
    }
  },
  getters: {
    isLoggedIn: state => !!state.token,
    authStatus: state => state.status,
    selectedPatrol: state => _.find(state.patrols,{id:state.selectedPatrolId}),
    patrols: state => state.patrols,
    user: state=> state.user,
    loadingCount: state=>state.loadingCount,
    loadingMessage: state=>state.loadingMessage
  }
})