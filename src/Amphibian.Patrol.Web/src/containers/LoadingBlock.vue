<template>
  <BlockUI v-if="loadingCount>0" :message="loadingMessage">
      <div class="sk-chase">
  <div class="sk-chase-dot"></div>
  <div class="sk-chase-dot"></div>
  <div class="sk-chase-dot"></div>
  <div class="sk-chase-dot"></div>
  <div class="sk-chase-dot"></div>
  <div class="sk-chase-dot"></div>
</div>
  </BlockUI>
</template>

<style>
.loading {
    color: #000000;
}

.loading-backdrop{
    background-color: #000000 !important;
    opacity: 0.6 !important;
}
</style>

<script>
import BlockUI from 'vue-blockui'

export default {
  name: 'LoadingBlock',
  BlockUI,
  computed: {
    loadingCount: function (){
        return this.$store.getters.loadingCount;
    },
    loadingMessage: function (){
        return this.$store.getters.loadingMessage ? this.$store.getters.loadingMessage : 'Loading';
    },
  },
  methods: {
    
  }
}
</script>
