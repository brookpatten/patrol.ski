<template>
  <div>
    <CRow class="justify-content-center">
      <CCol md="8" lg="8">
        <CCard>
          <CCardBody>
            <CRow>
              <CCol md="8">
                <h1 class="display-4">{{subdomain}}</h1>
              </CCol>
              <CCol md="4">
                
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
  name: 'Subdomain',
  components: {  },
  props: ['subdomain'],
  data () {
    return {
    }
  },
  methods:{
    
  }
}
</script>
