<template>
  <div class="c-app c-dark-theme">
    <div class="fullscreen-bg">
      <video autoplay muted loop class="fullscreen-bg__video">
          <source src="/snowmaking.mp4" type="video/mp4">
      </video>
    </div>
    <loading-block/>
    <router-view></router-view>
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

import LoadingBlock from './LoadingBlock'

export default {
  name: 'CenteredPublicContainer',
  components: {LoadingBlock}
}
</script>


