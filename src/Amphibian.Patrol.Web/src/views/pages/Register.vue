<template>
    <CContainer class="d-flex content-center min-vh-100">
      <CRow>
        <CCol>
          <CCard class="p-4">
            <CCardBody>
              <CForm v-on:submit.prevent="register">
                <h1>Register</h1>
                <p>Create your account</p>
                <p class="text-muted">Your information will not be shared outside of your patrol.  Ever.</p>
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
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
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
      this.$store.dispatch('register', data)
       .then(() => {
         if(this.$store.getters.patrols.length>0){
            this.$router.push('/');
          }
          else{
            this.$router.push({name:'NewPatrol'});
          }
        })
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
