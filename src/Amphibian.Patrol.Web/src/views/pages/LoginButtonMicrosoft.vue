<template>
    <div>
        <CButton v-if="clientId" size="lg" block color="dark" v-c-tooltip="'Login with Microsoft'" @click="login"><CIcon :content="$options.brands.cibMicrosoft"/></CButton>
    </div>
</template>

<script>
import * as Msal from "msal";
import { brandSet as brands } from '@coreui/icons'

export default {
  name:"LoginButtonMicrosoft",
  brands,
  components: {},
  data(){
    return {
      clientId:''
    }
  },
  methods: {
    loginApp(accessToken){
            this.$http.post('user/authenticate/microsoft',{accessToken:accessToken})
            .then(response => {
                console.log(response);
                this.$emit("authenticated", response);
            }).catch(response => {
                console.log(response);
            });
        },
    login(){
      let v= this;

      var msalConfig = {
          auth: {
              clientId: oauth2Configuration.microsoftClientId
          }
      };

      var msalInstance = new Msal.UserAgentApplication(msalConfig);

      msalInstance.handleRedirectCallback((error, response) => {
          console.log('error',error);
      });

      var loginRequest = {
          scopes: ["user.read"] // optional Array<string>
      };

      msalInstance.loginPopup(loginRequest)
      .then(response=>{
          msalInstance.acquireTokenSilent(loginRequest)
          .then(response => {
              console.log(response);
              v.loginApp(response.accessToken);
          })
          .catch(err => {
              // could also check if err instance of InteractionRequiredAuthError if you can import the class.
              if (err.name === "InteractionRequiredAuthError") {
                  return msalInstance.acquireTokenPopup(loginRequest)
                      .then(response => {
                          console.log(response);
                          v.loginApp(response.accessToken);
                      })
                      .catch(err => {
                          console.log(err);
                          // handle error
                      });
              }
              else
              {
                  console.log(err);
              }
          });
      })
      .catch(err=>{
          console.log(err);
      });

      
  }
  },
  mounted: function(){
      console.log('LoginButtonMicrosoft.vue');
  },
  beforeMount: function(){
    this.clientId= oauth2Configuration.microsoftClientId;
  }
}
</script>