import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import lodash from 'lodash'
import VueLodash from 'vue-lodash'

Vue.use(Vuex)
Vue.use(VueLodash, lodash)

var loadState = function(){
  console.log('Initializing state');
    var newState={
      sidebarShow: 'responsive',
      sidebarMinimize: false,
      status: '',
      token: '',
      user: null,
      patrols: [],
      selectedPatrolId: 0,
      loadingMessage: 'Loading',
      loadingCount: 0
    };

    var tryParseJson = function(json,defaultValue){
      if(!json)
      {
        console.log('json is null');
        return defaultValue;
      }
      else
      {
        try
        {
          return JSON.parse(json);
        }
        catch(err)
        {
          console.log(err.message);
          return defaultValue;
        }
      }
    };

    newState.token = localStorage.getItem('token') !=null ? localStorage.getItem('token') : '';

    newState.user= tryParseJson(localStorage.getItem('user'),null);
    newState.patrols=  tryParseJson(localStorage.getItem('patrols'),{patrols:[]}).patrols;
    newState.selectedPatrolId= localStorage.getItem('selectedPatrolId')!=null ? parseInt(localStorage.getItem('selectedPatrolId')) : 0;
    console.log(JSON.stringify(newState));
    return newState;
  };

const state = loadState();

const mutations = {
  initializeState(state){
    //loadState();
  },
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
    authenticate({commit},resp){
      console.log('authenticate',resp);
      return new Promise((resolve, reject) => {
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
    selectedPatrol: function(state){
      console.log('selectedpatrolid:'+state.selectedPatrolId);
      var patrol = _.find(state.patrols,function(s){
        return s.id==state.selectedPatrolId;
      });
      if(patrol){
        return patrol;
      }
      else{
        return {
          permissions:[],
          id:null,
          name:null,
          enableTraining:false,
          enableAnnouncements:false,
          enableEvents:false,
          enableScheduling:false,
          enableShiftSwaps:false,
          enableTimeClock:false,
          timeZone:false
        }
      }
    },
    patrols: state => state.patrols,
    user: state=> state.user,
    loadingCount: state=>state.loadingCount,
    loadingMessage: state=>state.loadingMessage
  }
})