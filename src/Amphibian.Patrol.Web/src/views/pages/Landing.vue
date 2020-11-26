<template>
  <div>
    <CRow>
      <CCol md="12" lg="12">
        <CCard>
          <CCardBody>
            <CJumbotron
                color="dark"
                text-color="white"
                border-color="dark"
              >
                <h1 class="display-3">Patrol<span style="color:#8C0C00">.Ski</span></h1>
                <p class="lead">
                  The app for ski patrol OET training.  Free for NSP patrols.
                </p>
            </CJumbotron>
          </CCardBody>
          <CCardFooter>
            <CButtonGroup class="float-right">
                <CButton
                  color="success"
                  :to="{name:'Register'}"
                > Register Now
                </CButton>
                <CButton color="info" :to="{name:'TestDrive'}">Test Drive</CButton>
              </CButtonGroup>
          </CCardFooter>
        </CCard>
      </CCol>
    </CRow>
    <CRow>
      <CCol md="6" lg="6">
        <CCard>
          <CCardHeader>For Patrol Administrators</CCardHeader>
          <CCardBody>
              <ul>
                <li>Manage multiple simultaneous training plans in a structured, consistent way that allows you to choose from an existing library of plans, customize them, or start from scratch.</li>
                <li>Fine grained control of which trainers can sign off on each requirement</li>
                <li>Connect to your scheduling / time-clock system to sync trainer shifts and allow trainees to sign up for training.  If you don't use a timekeeping system upload your schedule yourself.</li>
                <li>Manage training group sizes and help trainees find the trainers who can best help them.</li>
              </ul>
          </CCardBody>
        </CCard>
      </CCol>
      <CCol md="6" lg="6">
        <CCard>
          <CCardHeader>For Patrol Trainees</CCardHeader>
          <CCardBody>
              <ul>
                <li>Easily check progress on training assignments</li>
                <li>Receive notifications when sign-offs are completed</li>
                <li>View upcoming shifts filtered by which trainers can help the trainee progress</li>
              </ul>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
  </div>
</template>

<script>
export default {
  name: 'Landing'
}
</script>
