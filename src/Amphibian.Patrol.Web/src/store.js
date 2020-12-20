import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'
import lodash from 'lodash'
import VueLodash from 'vue-lodash'
import jwt_decode from 'jwt-decode'

Vue.use(Vuex)
Vue.use(VueLodash, lodash)

var loadState = function(){
  console.log('Initializing state');
    var newState={
      sidebarShow: 'responsive',
      sidebarMinimize: false,
      status: '',
      token: '',
      userId:0,
      patrols: [],
      selectedPatrolId: 0,
      loadingMessage: 'Loading',
      loadingCount: 0
    };

    newState.token = localStorage.getItem('token') !=null ? localStorage.getItem('token') : '';
    if(newState.token){
      var decoded = jwt_decode(newState.token);
      newState.patrols = JSON.parse(decoded.patrols);
      newState.userId = parseInt(decoded.uid);
    }
    newState.selectedPatrolId= localStorage.getItem('selectedPatrolId')!=null ? parseInt(localStorage.getItem('selectedPatrolId')) : 0;

    if((!newState.selectedPatrolId || _.find(newState.patrols,{id:newState.selectedPatrolId}) == null )
      && newState.patrols && newState.patrols.length>0){
        newState.selectedPatrolId = newState.patrols[0].id;
    }

    console.log(JSON.stringify(newState));
    return newState;
  };

const state = loadState();

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
  auth_success(state, jwt){
    state.status = 'success';
    state.token = jwt;
    var decoded = jwt_decode(jwt);
    state.patrols = JSON.parse(decoded.patrols);
    state.userId = parseInt(decoded.uid);

    //if no patrol is selected, or the selected patrol is no longer allowed, just pick the first one that IS allowed
    if(!state.selectedPatrolId
      && state.patrols 
      && state.patrols.length>0){
      state.selectedPatrolId = state.patrols[0].id;
    }
    else if(state.selectedPatrolId 
      && _.find(state.patrols,function(s){ return s.id==state.selectedPatrolId;}) == null 
      && state.patrols 
      && state.patrols.length>0){
      state.selectedPatrolId = state.patrols[0].id;
    }
    else if(!state.patrols 
      || state.patrols.length ==0 
      || _.find(state.patrols,function(s){ return s.id==state.selectedPatrolId;}) == null){
      //user has no access
      state.selectedPatrolId = 0;
    }
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
    authenticate({commit},jwt){
      console.log('authenticate',jwt);
      return new Promise((resolve, reject) => {
        commit('auth_success', jwt);
        localStorage.setItem('token', jwt);
        axios.defaults.headers.common['Authorization'] = 'Bearer ' + jwt;
        resolve(jwt);
      });
    },
    change_patrol({commit}, patrolId){
      return new Promise((resolve, reject) => {
        localStorage.setItem('selectedPatrolId', patrolId);
        commit('change_patrol', patrolId);
        resolve();
      });
    },
    logout({commit}){
      return new Promise((resolve, reject) => {
        commit('logout');
        localStorage.removeItem('token');
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
    userId: state=> state.userId,
    loadingCount: state=>state.loadingCount,
    loadingMessage: state=>state.loadingMessage,
    token: state=> state.token
  }
})