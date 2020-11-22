<template>
  <div class="d-flex align-items-center min-vh-100">
    <CContainer fluid>
      <CRow class="justify-content-center">
        <CCol md="6">
          <CCard class="mx-4 mb-0">
            <CCardBody class="p-4">
              <CForm v-on:submit.prevent="register">
                <h1>Register</h1>
                <p class="text-muted">Create your account</p>
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
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
    </CContainer>
  </div>
</template>

<script>
export default {
  name: 'Register',
  data(){
    return {
      first: "",
      last: "",
      email: "",
      password: "",
      password_confirmation: "",
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
        password: this.password
      };
      console.log("register",data);
      this.$store.dispatch('register', data)
       .then(() => this.$router.push('/dashboard'))
       .catch(err => 
        {
          this.processValidations(err.response.data.errors,this)
          console.log(err);
        });
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
