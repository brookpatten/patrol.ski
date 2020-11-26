<template>
  <div>
    <CRow>
      <CCol md="12" lg="12">
        <CJumbotron
              color="dark"
              text-color="white"
              border-color="dark"
            >
              <h1 class="display-3">Help & Support</h1>
              <p class="lead">
                Need help?
              </p>
          </CJumbotron>
      </CCol>
    </CRow>
    <CRow>
      <CCol md="6" lg="6">
        <CCard>
          <CCardHeader>Requesting Help</CCardHeader>
          <CCardBody>
            <p>The best way to get help is to <a href='https://github.com/brookpatten/patrol.ski/issues'>create an issue</a> on GitHub.
            You will need to create an account, but it will insure your issue does not get lost in an email inbox.</p>
            <p>If you would prefer to use email, you may contact help@patrol.ski.</p>
          </CCardBody>
        </CCard>
      </CCol>
      <CCol md="6" lg="6">
        <CCard>
          <CCardHeader>Comments and Feature Requests</CCardHeader>
          <CCardBody>
            <p>Patrol.Ski is open source, meaning anyone can freely contribute code, features, and fixes by submitting a <a href="https://github.com/brookpatten/patrol.ski/pulls">
            pull request</a> to the project on GitHub. If you have a developer on your patrol, asking them to contribute the feature you want may be the fastest way to see it implemented</p>
            <p>To request development of a new feature please <a href='https://github.com/brookpatten/patrol.ski/issues'>create an issue</a> on GitHub.</p>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
  </div>
</template>

<script>
export default {
  name: 'Help'
}
</script>
