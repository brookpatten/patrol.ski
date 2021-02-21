<template>
  <CContainer class="d-flex content-center min-vh-100">
    <CRow>
      <CCol>
        <CCardGroup>
          <CCard class="p-4">
            <CCardBody>
              <CForm @submit.prevent="login">
                <h1 class="display-4">Patrol<span style="color:#8C0C00">.Ski</span></h1>
                <p class="text-muted">Sign In to your account</p>
                <CInput
                  placeholder="Email"
                  autocomplete="email"
                  v-model="email"
                >
                  <template #prepend-content><CIcon name="cil-user"/></template>
                </CInput>
                <CInput
                  placeholder="Password"
                  type="password"
                  autocomplete="curent-password"
                  v-model="password"
                >
                  <template #prepend-content><CIcon name="cil-lock-locked"/></template>
                </CInput>
                <CRow>
                  <CCol col="12" v-if="error!=null" class="text-left">
                    <CAlert color="danger">{{error}}</CAlert>
                  </CCol>
                </CRow>
                <CRow>
                  <CCol col="12" v-if="message!=null" class="text-left">
                    <CAlert color="primary">{{message}}</CAlert>
                  </CCol>
                </CRow>
                <CRow>
                  <CCol col="6" class="text-left">
                    <CButton color="primary" class="px-4" type="submit">Login</CButton>
                  </CCol>
                  <CCol col="6" class="text-right">
                    <CButton color="link" class="px-0" v-on:click="reset">Forgot password?</CButton>
                    <CButton color="link" class="d-md-none" :to="{name:'Register'}">Register now!</CButton>
                    <CButton color="link" class="d-md-none" :to="{name:'TestDrive'}">Test Drive</CButton>
                  </CCol>
                </CRow>
                <CRow>
                  <CCol>
                    <hr/>
                  </CCol>
                  <CCol>
                    or
                  </CCol>
                  <CCol>
                    <hr/>
                  </CCol>
                </CRow>
                <CRow>
                  <CCol><login-button-google v-on:authenticated="afterLogin"></login-button-google></CCol>
                  <CCol><login-button-facebook v-on:authenticated="afterLogin"></login-button-facebook></CCol>
                  <CCol><login-button-microsoft v-on:authenticated="afterLogin"></login-button-microsoft></CCol>
                </CRow>
                <CRow><CCol><hr/></CCol></CRow>
                <CRow>
                  <CCol>
                    By registering for Patrol.ski you agree to the <a href='/#/terms'>Terms of use</a><br/>View the <a href='/#/privacy'>Privacy Policy</a>
                  </CCol>
                </CRow>
              </CForm>
            </CCardBody>
          </CCard>
          <CCard
            color="dark"
            text-color="white"
            class="text-center py-5 d-sm-down-none"
            body-wrapper
          >
            <h2>Sign up</h2>
            <p>Schedule, timeclock, training, announcements, calendar and notifications for ski patrol</p>
            <p>Free for Volunteer Patrols</p>
            <CButtonGroup class="active mt-3">
            <CButton
              color="success"
              :to="{name:'Register'}"
            > Register Now!
            </CButton>
            <CButton color="info" :to="{name:'TestDrive'}">Test Drive</CButton>
            </CButtonGroup>
          </CCard>
        </CCardGroup>
      </CCol>
    </CRow>
  </CContainer>
</template>

<script>
import { brandSet as brands } from '@coreui/icons'
import LoginButtonGoogle from './LoginButtonGoogle'
import LoginButtonFacebook from './LoginButtonFacebook'
import LoginButtonMicrosoft from './LoginButtonMicrosoft'
export default {
  name: 'Login',
  brands,
  components:{LoginButtonGoogle,LoginButtonFacebook,LoginButtonMicrosoft},
  data(){
      return {
        email : "",
        password : "",
        error: null,
        message: null
      }
    },
  methods: {
    afterLogin: function(){
      //the http interceptor automatically picks up the new jwt on the response 
      if(this.$store.getters.patrols.length>0){
        this.$router.push({name:'Home'})
      }
      else{
        this.$router.push({name:'NewPatrol'});
      }
    },
    login: function(){
      let email = this.email;
      let password = this.password;
      this.$store.dispatch('loading','Signing in');
      this.$http.post('user/authenticate',{email,password})
        .then(resp => {
          this.afterLogin();
        })
        .catch(err => {
          this.error = "Email or password is incorrect";
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    isValidEmail: function(email){
      if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email))
      {
        return (true)
      }
        return (false)
    },
    reset: function(){
      let email = this.email;
      if(this.isValidEmail(email)){
        this.$http.post('user/reset-password',{email}).then(response=>{
          this.message = "Check your email for a link to reset your password";
          this.error=null;
        }).catch(err=>{
          this.error = "Hmm, something went wrong";
          this.message=null;
        });
      }
      else{
        this.error="Please enter your email address";
        this.message=null;
      }
    }
  },
  mounted:function(){
    if(this.$route.query.jwt){
      //there is a minimal jwt in the query string, likely from an emailed link, attempt to auth using that
      this.$store.dispatch('loading','Signing in');
      this.$store.dispatch('authenticate',this.$route.query.jwt).then(p=>
      {
        //this can really be any authenticated rest call because we're really just using the token exchange that is built into the auth handler,
        //eg, bearer token is passed in on the request header, new token is returned on the response header
        this.$http.get('user')
        .then(resp => {
          this.$router.push({name:'Profile'})
        })
        .catch(err => {
          this.error = "The email/link has expired";
        }).finally(response=>this.$store.dispatch('loadingComplete'));
      });
    }
  }
}
</script>
