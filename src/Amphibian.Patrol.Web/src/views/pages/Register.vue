<template>
    <CContainer class="d-flex content-center min-vh-100">
      <CRow>
        <CCol>
          <CCard class="p-4">
            <CCardBody>
              <CRow><CCol>
              <CForm v-on:submit.prevent="register">
                <h1>Register</h1>
                <p>Create your account</p>
                <p class="text-muted">Your information will not be shared outside of your patrol.</p>
                <p class="text-muted">Personal details are used to identify you to your patrol and to send you notifications (which you may disable)</p>
                <CInput
                  placeholder="First Name"
                  autocomplete="first"
                  v-model="firstname"
                  :isValid="validation.validated ? validation.firstname == null : null"
                  :invalidFeedback="validation.firstname"
                >
                </CInput>
                <CInput
                  placeholder="Last Name"
                  autocomplete="last"
                  v-model="lastname"
                  :isValid="validation.validated ? validation.lastname == null : null"
                  :invalidFeedback="validation.lastname"
                >
                </CInput>
                <CInput
                  placeholder="Email"
                  autocomplete="email"
                  prepend="@"
                  v-model="email"
                  :isValid="validation.validated ? validation.email == null : null"
                  :invalidFeedback="validation.email"
                />
                <CInput
                  placeholder="NSP #"
                  prepend="#"
                  v-model="nspNumber"
                  :isValid="validation.validated ? validation.nspNumber == null : null"
                  :invalidFeedback="validation.nspNumber"
                />
                <CInput
                  placeholder="Password"
                  type="password"
                  autocomplete="new-password"
                  v-model="password"
                  :isValid="validation.validated ? validation.password == null : null"
                  :invalidFeedback="validation.password"
                >
                  <template #prepend-content><CIcon name="cil-lock-locked"/></template>
                </CInput>
                <CInput
                  placeholder="Repeat password"
                  type="password"
                  autocomplete="new-password"
                  class="mb-4"
                  v-model="password_confirmation"
                  :isValid="password_confirmation==null || password_confirmation=='' ? null : password_confirmation == password"
                  invalidFeedback="Passwords do not match"
                >
                  <template #prepend-content><CIcon name="cil-lock-locked"/></template>
                </CInput>
                <CButton color="success" block type="submit">Create Account</CButton>
              </CForm>
              </CCol></CRow>
              <CRow><CCol><hr/></CCol></CRow>
              <CRow>
                  <CCol>
                    By registering for Patrol.ski you agree to the <a href='/#/terms'>Terms of use</a><br/>View the <a href='/#/privacy'>Privacy Policy</a>
                  </CCol>
              </CRow>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
      <CRow><CCol><hr/></CCol></CRow>
    </CContainer>
</template>

<script>
export default {
  name: 'Register',
  data(){
    return {
      firstname: "",
      lastname: "",
      email: "",
      password: "",
      password_confirmation: "",
      nspNumber:"",
      validation: {
        validated: false
      }
    }
  },
  methods: {
    register: function(){
      
      let data = {
        firstname: this.firstname,
        lastname: this.lastname,
        email: this.email,
        password: this.password,
        nspNumber: this.nspNumber
      };
      console.log("register",data);
      this.$store.dispatch('loading','Registering');
      this.$http.post('user/register',data)
        .then(resp => {
          //the http interceptor automatically picks up the new jwt on the response 
          this.$router.push({name:'NewPatrol'});
        })
        .catch(err => {
          this.processValidations(err.response.data.errors,this)
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    processValidations: function(errors, vm) {
      vm.validation.validated=true;
      if(errors) {
          _.each(errors, function(stateErrors, field) {
              var message = "";
              if(stateErrors)
              {
                  _.each(stateErrors, function (m, stateError) {
                      message += m+" ";
                  });
                  vm.$set(vm.validation, field, message);
              }
          });
      }
    }
  }
}
</script>
