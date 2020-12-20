<template>
  <CContainer class="d-flex content-center min-vh-100">
  </CContainer>
</template>

<script>
export default {
  name: 'TestDrive',
  data(){
      return {
      }
    },
  methods: {
    throwaway: function(){
      this.$store.dispatch('loading','Logging In');
      this.$http.post('user/authenticate-throwaway')
        .then(resp => {
          //the http interceptor automatically picks up the new jwt on the response
          this.$router.push({name:'Home'}) 
        })
        .catch(err => {
          this.error = err;
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
  },
  mounted(){
      this.throwaway();
  }
}
</script>
