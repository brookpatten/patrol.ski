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
                  placeholder="First"
                  autocomplete="first"
                  v-model="first"
                >
                </CInput>
                <CInput
                  placeholder="Last"
                  autocomplete="last"
                  v-model="last"
                >
                </CInput>
                <CInput
                  placeholder="Email"
                  autocomplete="email"
                  prepend="@"
                  v-model="email"
                />
                <CInput
                  placeholder="Password"
                  type="password"
                  autocomplete="new-password"
                  v-model="password"
                >
                  <template #prepend-content><CIcon name="cil-lock-locked"/></template>
                </CInput>
                <CInput
                  placeholder="Repeat password"
                  type="password"
                  autocomplete="new-password"
                  class="mb-4"
                  v-model="password_confirmation"
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
      password_confirmation: ""
    }
  },
  methods: {
    register: function(){
      
      let data = {
        first: this.first,
        last: this.last,
        email: this.email,
        password: this.password
      };
      console.log("register",data);
      this.$store.dispatch('register', data)
       .then(() => this.$router.push('/dashboard'))
       .catch(err => console.log(err));
    }
  }
}
</script>
