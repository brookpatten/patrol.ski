<template>
  <div>
    <CRow class="justify-content-center">
      <CCol md="8" lg="8">
        <CCard>
          <CCardHeader>
              Account Deletion
          </CCardHeader>
          <CCardBody>
            <CRow>
              <CCol>
                  <p>To delete your personal information, log in and visit <CButton color="link" :to="{name:'Profile'}">Your Profile</CButton></p>
                  <img src="delete-profile.png" alt="profile link" style="width:100%"/>
                  <p>Then Click "Forget Me"</p>
                  <img src="delete-forgetme.png" alt="forget me link" style="width:100%"/>
                  <p>Lastly, click "Yes I am sure, Forget Me"</p>
                  <img src="delete-confirm.png" alt="forget me confirmation" style="width:100%"/>
              </CCol>
            </CRow>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
  </div>
</template>
<script>
export default {
  name: 'Delete',
  data(){
    return {
      
    }
  },
  methods: {
    
    
  }
}
</script>
