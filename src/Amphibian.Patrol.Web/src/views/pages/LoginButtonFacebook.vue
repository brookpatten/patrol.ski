<template>
    <div>
        <CButton v-if="appId" size="lg" :disabled="!ready" block color="dark" @click="login" v-c-tooltip="'Login with facebook'"><CIcon :content="$options.brands.cibFacebook"/></CButton>
    </div>
</template>

<script>
import facebookLogin from 'facebook-login';
import { brandSet as brands } from '@coreui/icons'

const fbApi = facebookLogin({ appId: oauth2Configuration.facebookAppId });

export default {
  name:"LoginButtonFacebook",
  brands,
  components: {},
  data(){
    return {
        ready:false,
        appId:''
    }
  },
  methods: {
    login(){
      fbApi.login().then((response) => {
        console.log(response);
        if(response.status === 'connected')
        {
          this.$http.post('user/authenticate/facebook',{accessToken:response.authResponse.accessToken})
              .then(response => {
                  var result = response.data;
                  console.log(response);
                  this.$emit("authenticated", response);
              }).catch(response => {
                  console.log(response);
              });
        }
      });
    }
  },
  beforeMount: function(){
      this.appId = oauth2Configuration.facebookAppId;
      fbApi.whenLoaded().then(() => this.ready=true);
      console.log('fb ready');
  },
  mounted: function(){
      console.log('LoginButtonFacebook.vue');
  }
}
</script>