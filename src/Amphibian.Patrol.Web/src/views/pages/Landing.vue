<template>
  <div>
    <CRow class="justify-content-center">
      <CCol md="8" lg="8">
        <CCard>
          <CCardBody>
            <CRow>
              <CCol md="8">
                <h1 class="display-4">Patrol<span style="color:#8C0C00">.Ski</span></h1>
                <p class="lead">
                  An app for ski patrol.  Free for Volunteer patrols.
                </p>
              </CCol>
              <CCol md="4">
                <CButtonGroup class="float-right">
                  <CButton
                    color="success"
                    :to="{name:'Register'}"
                  > Register
                  </CButton>
                  <CButton color="info" :to="{name:'TestDrive'}">Test Drive</CButton>
                  <CButton color="primary" :to="{name:'Login'}">Sign in</CButton>
                </CButtonGroup>
              </CCol>
            </CRow>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
    <CRow class="justify-content-center">
      <CCol md="8" lg="8">
        <CCard>
          <CCardBody>
            <CRow>
              <CCol md="4" lg="4">
                <CRow><CCol>
                <img src="brook.jpg" alt="author's profile pic" class="img-fluid rounded"/>
                </CCol></CRow>
                <CRow><CCol>
                  <br/>
                  <CAlert color="info">Take a tour of the app below, or click "Test Drive" to dive right in.</CAlert>
                </CCol></CRow>
              </CCol>
              <CCol>
                  <p>Hi, My name is Brook and I am a volunteer ski patroller in Central division, Ohio region</p>
                  <p>I'm fairly new to patrolling, but I'm an experienced software developer.  After my first season I had 
                    some ideas for tools I could make that might be helpful to volunteer patrols like ours.  
                    I quickly found that patrollers I spoke to had different ideas about what kinds of apps their patrol needed.
                  </p>
                  <p>
                    From those ideas, this project has grown.  Realizing that patrols from different host areas have varying needs and resources, <strong>all functions are optional</strong>.
                    You can enable the functions you want, and disable the ones you don't.  Maybe your area already has a good scheduling/time solution and you like the training functions.
                    Maybe you prefer to use social media for your calendar and notifications.  That's perfectly fine and this App won't get in your way.
                  </p>
                  <p>
                    I intend for this to be a <strong>free app for volunteer patrols</strong>, and for the source code to be freely available as well.
                    If you have questions, comments, problems, or feature requests feel free to shoot me an email <strong>help@patrol.ski</strong> or visit the
                    <a href='https://github.com/brookpatten/patrol.ski'> GitHub project</a>
                  </p>
                  <p>
                    By registering for Patrol.ski you agree to the <a href='/#/terms'>Terms of use</a><br/>View the <a href='/#/privacy'>Privacy Policy</a>
                  </p>
              </CCol>
            </CRow>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
    <CRow class="justify-content-center">
      <CCol md="8" lg="8">
        <CCard color="dark">
        <CTabs add-tab-classes="mt-1">
          <CTab active>
            <template slot="title">
              <CIcon name="cil-calendar"/> Scheduling
            </template>
            <CRow>
              <CCol md="12" lg="6">
                <CCard class="justify-content-center">
                  <CCardBody>
                    <img src="feature-schedule-calendar.png" alt="announcement screenshot" style="width:250px" @click="openGallery(12)"/>
                  </CCardBody>
                  <CCardFooter>
                    Create your schedule quickly.  Schedule every shift individually or create "cookie cutter" shifts
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-schedule-swap.png" alt="announcement screenshot" style="width:250px" @click="openGallery(15)"/>
                  </CCardBody>
                  <CCardFooter>
                    Allow your patrol to swap & pick up shifts, but administrators retain final approval
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-schedule-available.png" alt="available scheduling screenshot" style="width:250px" @click="openGallery(23)"/>
                  </CCardBody>
                  <CCardFooter>
                    Schedule Available shifts which patrollers can claim to schedule themselves
                  </CCardFooter>
                </CCard>
              </CCol>
            </CRow>
          </CTab>
          <CTab>
            <template slot="title">
              <CIcon name="cil-spreadsheet"/> Training
            </template>
            <CRow>
              <CCol md="12" lg="6">
                <CCard class="justify-content-center">
                  <CCardBody>
                    <img src="feature-assignments.png" alt="announcement screenshot" style="width:250px" @click="openGallery(2)"/>
                  </CCardBody>
                  <CCardFooter>
                    Assign training to members of your patrol and track their progress
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-training-plans.png" alt="announcement screenshot" style="width:250px" @click="openGallery(16)"/>
                  </CCardBody>
                  <CCardFooter>
                    Manage multiple simultaneous training plans in a structured, consistent way
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-edit-plan.png" alt="announcement screenshot" style="width:250px" @click="openGallery(6)"/>
                  </CCardBody>
                  <CCardFooter>
                    Choose from an existing library of training plans, customize them, or start from scratch.
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-edit-section.png" alt="announcement screenshot" style="width:250px" @click="openGallery(7)"/>
                  </CCardBody>
                  <CCardFooter>
                    Fine grained control of which trainers can sign off on each requirement
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-available-training-shifts.png" alt="announcement screenshot" style="width:250px" @click="openGallery(3)"/>
                  </CCardBody>
                  <CCardFooter>
                    Allow trainees to quickly find trainers that can help them by joining trainer credentials, training plans, and the schedule
                  </CCardFooter>
                </CCard>
              </CCol>
            </CRow>
          </CTab>
          <CTab>
            <template slot="title">
              <CIcon name="cil-calendar"/> Timeclock
            </template>
            <CRow>
              <CCol md="12" lg="6">
                <CCard class="justify-content-center">
                  <CCardBody>
                    <img src="feature-timeclock-dashboard.png" alt="Dashboard screenshot" style="width:250px" @click="openGallery(20)"/>
                  </CCardBody>
                  <CCardFooter>
                    Clock In/Out, View who's currently clocked in, and who's late gone early (if also using scheduling)
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-timeclock-days.png" alt="Timecard screenshot" style="width:250px" @click="openGallery(21)"/>
                  </CCardBody>
                  <CCardFooter>
                    Report hours per day per person
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-timeclock-missing.png" alt="Missed shift screenshot" style="width:250px" @click="openGallery(22)"/>
                  </CCardBody>
                  <CCardFooter>
                    Report late arrival, early leaving, and missed shifts (if also using scheduling)
                  </CCardFooter>
                </CCard>
              </CCol>
            </CRow>
          </CTab>
          <CTab>
              <template slot="title">
                <CIcon name="cil-task"/> Work Tracking
              </template><CRow>
              <CCol md="12" lg="6">
                <CCard class="justify-content-center">
                  <CCardBody>
                    <img src="feature-one-off-work-item.png" alt="announcement screenshot" style="width:250px" @click="openGallery(24)"/>
                  </CCardBody>
                  <CCardFooter>
                    Create one-off work and track to completion.  Allow work to be marked complete by anyone, any one of a group of assigned patrollers, all of a group, or administrators only.
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-incomplete-work.png" alt="announcement screenshot" style="width:250px" @click="openGallery(25)"/>
                  </CCardBody>
                  <CCardFooter>
                    Patrollers quickly see work assigned to them directly on their dashboard
                  </CCardFooter>
                </CCard>
              </CCol>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-workitem-list.png" alt="announcement screenshot" style="width:250px" @click="openGallery(26)"/>
                  </CCardBody>
                  <CCardFooter>
                    Audit completed work and check on assigned and upcoming work in a central addministrative console
                  </CCardFooter>
                </CCard>
              </CCol>

              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-timed-workitem.png" alt="announcement screenshot" style="width:250px" @click="openGallery(27)"/>
                  </CCardBody>
                  <CCardFooter>
                    Create timed recurring work (daily, weekly, monthly, yearly) such as equipment checks or paperwork deadlines
                  </CCardFooter>
                </CCard>
              </CCol>

              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-shift-workitem.png" alt="announcement screenshot" style="width:250px" @click="openGallery(29)"/>
                  </CCardBody>
                  <CCardFooter>
                    If you're also using scheduling, create recurring work that occurs at a specific time on specific shifts such as equipment checks for morning shift or run sweeps in evening shfits.
                  </CCardFooter>
                </CCard>
              </CCol>

              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-recurring-workitem-list.png" alt="announcement screenshot" style="width:250px" @click="openGallery(29)"/>
                  </CCardBody>
                  <CCardFooter>
                    Manage recurring work in a central administrative console
                  </CCardFooter>
                </CCard>
              </CCol>
            </CRow>
          </CTab>
          <CTab>
              <template slot="title">
                <CIcon name="cil-comment-square"/> Communication
              </template>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-announcement.png" alt="announcement screenshot" style="width:300px" @click="openGallery(0)"/>
                  </CCardBody>
                  <CCardFooter>
                    Create announcements and calendar events, pin them to the dashboard and email the entire patrol
                  </CCardFooter>
                </CCard>
              </CCol>
          </CTab>
          <CTab>
              <template slot="title">
                <CIcon name="cil-phone"/> Mobile
              </template>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-mobile.png" alt="announcement screenshot" style="height:200px" @click="openGallery(19)"/>
                  </CCardBody>
                  <CCardFooter>
                    Use the app with a PC, tablet, or mobile
                  </CCardFooter>
                </CCard>
              </CCol>
          </CTab>
          <CTab>
              <template slot="title">
                <CIcon name="cil-people"/> Public Site
              </template>
              <CCol md="12" lg="6">
                <CCard>
                  <CCardBody>
                    <img src="feature-public.png" alt="announcement screenshot" style="height:200px" @click="openGallery(30)"/>
                  </CCardBody>
                  <CCardFooter>
                    Enable a public-facing site to share selected information, events, and announcements with the public.
                  </CCardFooter>
                </CCard>
              </CCol>
          </CTab>
        </CTabs>
      </CCard>
    </CCol>
  </CRow>
  <CRow>
    <CCol>
      <LightBox 
      :media="images" 
      ref="lightbox"
      :show-caption="true"
      :show-light-box="false"
    ></LightBox>
    </CCol>
  </CRow>
  </div>
</template>

<script>
import LightBox from 'vue-image-lightbox'

import 'vue-image-lightbox/dist/vue-image-lightbox.min.css';

export default {
  name: 'Landing',
  components: { LightBox },
  data () {
    return {
      images:[
        {
          src:'feature-announcement.png',
          thumb:'feature-announcement.png',
          caption:'Post & email announcements'
        },
        {
          src:'feature-assignment.png',
          thumb:'feature-assignment.png',
          caption:'View Training Assignment Progress'
        },
        {
          src:'feature-assignments.png',
          thumb:'feature-assignments.png',
          caption:'Manage and monitor training across your entire patrol'
        },
        {
          src:'feature-available-training-shifts.png',
          thumb:'feature-available-training-shifts.png',
          caption:'Easily find trainers who can help accomplish your training assignments'
        },
        {
          src:'feature-edit-announcement.png',
          thumb:'feature-edit-announcement.png',
          caption:'Compose announcements and broadcast emails'
        },
        {
          src:'feature-edit-patrol.png',
          thumb:'feature-edit-patrol.png',
          caption:'Use only the functionality you want, disable the things you do not want'
        },
        {
          src:'feature-edit-plan.png',
          thumb:'feature-edit-plan.png',
          caption:'Start with a patrol.ski training plan and modify it for your needs, or start over from scratch'
        },
        {
          src:'feature-edit-section.png',
          thumb:'feature-edit-section.png',
          caption:'Control who can provide training sign-offs for each section of a training plan using groups'
        },
        {
          src:'feature-groups.png',
          thumb:'feature-groups.png',
          caption:'Create groups of people to more easily manage training, scheduling etc'
        },
        {
          src:'feature-my-assignments.png',
          thumb:'feature-my-assignments.png',
          caption:'View your training assignments and progress'
        },
        {
          src:'feature-people.png',
          thumb:'feature-people.png',
          caption:'Manage people in your patrol'
        },
        {
          src:'feature-plan.png',
          thumb:'feature-plan.png',
          caption:'Design structured training plans'
        },
        {
          src:'feature-schedule-calendar.png',
          thumb:'feature-schedule-calendar.png',
          caption:'Create and manage the schedule for your patrol'
        },
        {
          src:'feature-schedule-custom.png',
          thumb:'feature-schedule-custom.png',
          caption:'Schedule every shift time and person'
        },
        {
          src:'feature-schedule-shift.png',
          thumb:'feature-schedule-shift.png',
          caption:'Or use pre-set shifts and groups to quickly build your schedule'
        },
        {
          src:'feature-schedule-swap.png',
          thumb:'feature-schedule-swap.png',
          caption:'Allow your patrol to release, swap, and claim shifts, Administrators retain final approval of changes'
        },
        {
          src:'feature-training-plans.png',
          thumb:'feature-training-plans.png',
          caption:'Create and manage multiple simultaneous structured training plans'
        },
        {
          src:'feature-training-progress.png',
          thumb:'feature-training-progress.png',
          caption:'Monitor training progress across your entire patrol'
        },
        {
          src:'feature-upcoming-training-shifts.png',
          thumb:'feature-upcoming-training-shifts.png',
          caption:'View shifts for which you have signed up to train and with who'
        },
        {
          src:'feature-mobile.png',
          thumb:'feature-mobile.png',
          caption:'Works on pc, tablet, or mobile'
        },
        {
          src:'feature-timeclock-dashboard.png',
          thumb:'feature-timeclock-dashboard.png',
          caption:'Timeclock & Dashboard'
        },
        {
          src:'feature-timeclock-days.png',
          thumb:'feature-timeclock-days.png',
          caption:'Report daily hours'
        },
        {
          src:'feature-timeclock-missing.png',
          thumb:'feature-timeclock-missing.png',
          caption:'Report missed/late/mia shifts if both timeclock and scheduling are used'
        },
        {//23
          src:'feature-schedule-available.png',
          thumb:'feature-schedule-available.png',
          caption:'Schedule shifts as Available for users to claim'
        },
        {
          src:'feature-one-off-work-item.png',
          thumb:'feature-one-off-work-item.png',
          caption:'Create one-off work at-will and track through to completion'
        },
        {
          src:'feature-incomplete-work.png',
          thumb:'feature-incomplete-work.png',
          caption:'Patrollers are shown assigned work directly on the dashboard'
        },
        {
          src:'feature-workitem-list.png',
          thumb:'feature-workitem-list.png',
          caption:'Audit completed work and view upcoming work'
        },
        {
          src:'feature-timed-workitem.png',
          thumb:'feature-timed-workitem.png',
          caption:'Create recurring work that occur on a timed interval (daily/weekly/monthly/yearly) such as equipment checks or paperwork deadlines'
        },
        {
          src:'feature-shift-workitem.png',
          thumb:'feature-shift-workitem.png',
          caption:'Create recurring work that always occur on a particular shift such as equipment checks or sweeps'
        },
        {
          src:'feature-recurring-workitem-list.png',
          thumb:'feature-recurring-workitem-list.png',
          caption:'Manage all your recurring work in a central location'
        }
        ,
        {
          src:'feature-public.png',
          thumb:'feature-public.png',
          caption:'Enable a public-facing site to share selected events and announcements'
        }
      ],
    }
  },
  methods:{
    openGallery: function(index){
      console.log('open '+index);
      this.$refs.lightbox.showImage(index);
    }
  }
}
</script>
