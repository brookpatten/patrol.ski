<template>
  <CContainer class="d-flex content-center min-vh-100">
    <CRow>
      <CCol>
        <CCardGroup>
          <CCard class="p-4">
            <CCardBody>
              <CForm @submit.prevent="login">
                <h1>Login</h1>
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
                  </CCol>
                </CRow>
              </CForm>
            </CCardBody>
          </CCard>
          <CCard
            color="primary"
            text-color="white"
            class="text-center py-5 d-sm-down-none"
            body-wrapper
          >
            <h2>Sign up</h2>
            <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</p>
            <CButton
              color="primary"
              class="active mt-3"
               :to="{name:'Register'}"
            >
              Register Now!
            </CButton>
          </CCard>
        </CCardGroup>
      </CCol>
    </CRow>
  </CContainer>
</template>

<script>
export default {
  name: 'Login',
  data(){
      return {
        email : "",
        password : "",
        error: null,
        message: null
      }
    },
  methods: {
    login: function(){
      let email = this.email;
      let password = this.password;
      this.$store.dispatch('login',{email,password})
        .then(()=>this.$router.push('/'))
        .catch(err => {
          console.log(err);
          this.error = "Username or password is incorrect";
        });
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
          this.message = "We sent you an email with a link to reset your password";
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
  }
}
</script>
