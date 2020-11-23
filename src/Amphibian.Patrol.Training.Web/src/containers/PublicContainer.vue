<template>
  <div class="c-app c-dark-theme" :class="{ 'c-dark-theme': $store.state.darkMode }">
    <div class="fullscreen-bg">
      <video autoplay muted loop class="fullscreen-bg__video">
          <source src="/snowmaking.mp4" type="video/mp4">
      </video>
    </div>
    <CWrapper>
      <TheHeader/>
      <div class="c-body">
        <main class="c-main">
          <CContainer fluid>
            <loading-block/>
            <transition name="fade" mode="out-in">
              <router-view :key="$route.path"></router-view>
            </transition>
          </CContainer>
        </main>
      </div>
      <TheFooter/>
    </CWrapper>
  </div>
</template>

<style>
@media (max-width: 767px) {
    .fullscreen-bg__video {
        display: none;
    }
    .fullscreen-bg {
        background: url('/snowmaking.png');
        background-size: cover;
        /*-webkit-filter: blur(5px);
        -moz-filter: blur(5px);
        -o-filter: blur(5px);
        -ms-filter: blur(5px);
        filter: blur(5px);*/
    }
  }
  
.fullscreen-bg__video {
    position: fixed;
    right: 0;
    bottom: 0;
    min-width: 100%;
    min-height: 100%;
    -webkit-filter: blur(5px);
    -moz-filter: blur(5px);
    -o-filter: blur(5px);
    -ms-filter: blur(5px);
    filter: blur(5px);
  }
  .fullscreen-bg {
    position: fixed;
    right: 0;
    bottom: 0;
    min-width: 100%;
    min-height: 100%;
    overflow: hidden;
  }
</style>

<script>

import TheHeader from './TheHeader'
import TheFooter from './TheFooter'
import LoadingBlock from './LoadingBlock'

export default {
  name: 'PublicContainer',
  components: {
    TheHeader,TheFooter,LoadingBlock
  },
  computed: {
    user: function (){
        return this.$store.getters.user.id ? this.$store.getters.user : null;
    }
  },
  methods: {
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    }
  }
}
</script>


