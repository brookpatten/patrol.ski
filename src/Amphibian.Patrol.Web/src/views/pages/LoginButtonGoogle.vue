<template>
    <div>
        <CButton v-google-signin-button="clientId" v-if="clientId" size="lg" block color="dark" v-c-tooltip="'Login with Google'"><CIcon :content="$options.brands.cibGoogle"/></CButton>
    </div>
</template>

<script>
import GoogleSignInButton from 'vue-google-signin-button-directive'
import { brandSet as brands } from '@coreui/icons'

export default {
  name:"LoginButtonGoogle",
  brands,
  components: {GoogleSignInButton},
  data(){
    return {
        clientId : oauth2Configuration.googleClientId
    }
  },
  methods: {
    OnGoogleAuthSuccess (idToken) {
      // Receive the idToken and make your magic with the backend
      console.log(idToken);

      this.$http.post('user/authenticate/google',{id_token:idToken})
                .then(response => {
                    console.log(response);

                    this.$emit("authenticated", response);
                }).catch(response => {
                    console.log(response);
                })
    },
    OnGoogleAuthFail (error) {
      console.log(error)
    }
  },
  mounted: function(){
      console.log('LoginButtonGoogle.vue');
  },
  beforeMount: function(){
      this.clientId = oauth2Configuration.googleClientId;
  },
}
</script>