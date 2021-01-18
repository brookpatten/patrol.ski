<template>
    <div>
      <CRow>
        <CCol>
          <CForm @submit.prevent="save">
          <CCard>
            <CCardHeader>
              <slot name="header">
                <CIcon name="cil-user"/>
              </slot>
            </CCardHeader>
            <CCardBody>
                <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
                <CInput
                label="First Name"
                v-model="user.firstName"
                :invalidFeedback="validationErrors.FirstName ? validationErrors.FirstName.join() : 'Invalid'"
                :isValid="validated ? validationErrors.FirstName==null : null"
                />
                <CInput
                label="Last Name"
                v-model="user.lastName"
                :invalidFeedback="validationErrors.LastName ? validationErrors.LastName.join() : 'Invalid'"
                :isValid="validated ? validationErrors.LastName==null : null"
                />
                <!--validationErrors.Email != null ? validationErrors.Email.join() : null-->
                <CInput
                label="Email"
                v-model="user.email"
                disabled
                />
                <CInput
                label="NSP #"
                v-model="user.nspNumber"
                :invalidFeedback="validationErrors.NspNumber ? validationErrors.NspNumber.join() : 'Invalid'"
                :isValid="validated ? validationErrors.NspNumber==null : null"
                />
                
                <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="user.allowEmailNotifications"/><label>Email notifications</label>

            </CCardBody>
            <CCardFooter>
                <CButtonGroup>
                  <CButton v-if="!showDelete" @click="showDelete=true" color="warning">Forget Me</CButton>
                  <CButton v-if="showDelete" @click="showDelete=false" color="success">Oops, Nevermind</CButton>
                  <CButton v-if="showDelete" @click="deleteMe" color="danger">Yes, I am Sure, Forget me.</CButton>
                </CButtonGroup>
                <CButtonGroup class="float-right">
                  <CButton type="submit" color="primary">Save</CButton>
                </CButtonGroup>
            </CCardFooter>
          </CCard>
          </CForm>
        </CCol>
      </CRow>
      <CRow>
        <CCol>
          <CForm @submit.prevent="changePassword">
          <CCard>
            <CCardHeader>
              <slot name="header">
                <CIcon name="cil-user"/> Change Password
              </slot>
            </CCardHeader>
            <CCardBody>
                <CRow><CCol><CAlert color="info" v-if="reset.message">{{reset.message}}</CAlert></CCol></CRow>
                <CRow>
                  <CCol>
                    <CInput
                    label="Password"
                    v-model="reset.password"
                    />
                  </CCol>
                  <CCol>
                    <CInput
                    label="Repeat Password"
                    v-model="reset.repeatPassword"
                    />
                  </CCol>
                </CRow>
            </CCardBody>
            <CCardFooter>
                <CButtonGroup class="float-right">
                  <CButton type="submit" color="primary" v-if="reset.password.length>5 && reset.password == reset.repeatPassword">Change</CButton>
                </CButtonGroup>
            </CCardFooter>
          </CCard>
          </CForm>
        </CCol>
      </CRow>
    </div>
</template>

<script>

export default {
  name: 'Profile',
  components: {
  },
  data () {
    return {
      user:{},
      validationMessage:'',
      validationErrors:{},
      validated:false,
      showDelete:false,
      reset:{password:'',repeatPassword:'',message:null}
    }
  },
  methods: {
    getUser() {
      this.$store.dispatch('loading','Loading...');
          this.$http.get('user')
            .then(response => {
                this.user = response.data;
                console.log(response);
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));    
    },
    save(){
      this.$store.dispatch('loading','Saving...');
        this.$http.put('user',this.user)
          .then(response=>{
            this.$router.push({name:'Home'});
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    changePassword(){
      let reset = this.reset;
      this.$store.dispatch('loading','Changing Password...');
        this.$http.post('user/password',{password:this.reset.password})
          .then(response=>{
            reset.message="Password Changed";
          }).catch(response=>{
            reset.message="Password Change Failed";
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    deleteMe(){
      this.$store.dispatch('loading','Deleting...');
        this.$http.delete('user')
          .then(response=>{
            this.$store.dispatch('logout')
              .then(()=>this.$router.push({name:"Login"}))
              .catch(err => console.log(err));
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    isValid(obj,field){
      if(obj!=null){
        if(field!=null){
          return false;
        }
        else{
          return true;
        }
      }
      else{
        return false;
      }
    }
  },
  computed: {
    
  },
  watch: {
    
  },
  mounted: function(){
    this.getUser();
  }
}
</script>
