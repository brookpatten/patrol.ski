<template>
  <router-view></router-view>
</template>

<script>
export default {
  name: 'App',
  created: function () {
    var appComponent = this;
    this.$http.interceptors.response.use(undefined, function (err) {
      return new Promise(function (resolve, reject) {
        if (err.response.status === 401 && err.response.config && !err.response.config.__isRetryRequest) {
          //this occurs when the token used is no longer valid.  kick the user out to the login page
          appComponent.$store.dispatch('logout');
          appComponent.$router.push({name:'Login'});
          resolve();
        }
        throw err;
      });
    });
  }
}
</script>

<style lang="scss">
  // Import Main styles for this application
  @import 'assets/scss/style';
</style>
