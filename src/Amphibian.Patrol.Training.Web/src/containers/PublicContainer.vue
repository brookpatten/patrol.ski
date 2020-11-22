<template>
  <div class="c-app">
    <video autoplay muted loop id="backgroundVideo">
        <source src="/snowmaking.mp4" type="video/mp4">
    </video>
    <router-view></router-view>
  </div>
</template>

<style>
@media (max-width: 767px) {
    #backgroundVideo {
        display: none;
    }
    body {
        background: url('/snowmaking.png');
        background-size: cover;
    }
  }
  
#backgroundVideo {
    position: fixed;
    right: 0;
    bottom: 0;
    min-width: 100%;
    min-height: 100%;
  }
</style>

<script>

export default {
  name: 'PublicContainer',
  components: {
  }
}
</script>


