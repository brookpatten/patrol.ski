<template>
  <div>
    <CRow>
      <CCol md="12" lg="12">
        <CCard>
          <CCardBody>
            <CRow>
              <CCol md="8">
                <h1 class="display-4">Patrol<span style="color:#8C0C00">.Ski</span></h1>
                <p class="lead">
                  An app for ski patrol.  Free for NSP patrols.
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
                <img src="brook.jpg" alt="author's profile pic" class="img-fluid rounded"/>
              </CCol>
              <CCol>
                  <p>Hi, My name is Brook and I am a volunteer ski patroller in Central division, Ohio region</p>
                  <p>I'm fairly new to patrolling, but I'm an experienced software developer.  After my first season I had 
                    some ideas for tools I could make that might be helpful to volunteer ski patrols like ours.  
                    I quickly found that patrollers I spoke to online had different ideas about what kinds of apps their patrol needed.
                  </p>
                  <p>
                    From those ideas this project has grown.  It now includes OET Training plans and trainee progress tracking, trainer/trainee signups, 
                    scheduling, shift swapping, event calendar, announcements, and email notifications for all of the above.  None of these features are forced on you, if you're only interested in one 
                    part, just use that part and turn the rest off.
                  </p>
                  <p>
                    I intend for this to be a free app for volunteer patrols, and for the source code to be freely available as well.
                    If you have questions, comments, problems, or feature requests feel free to shoot me an email help@patrol.ski or visit the
                    <a href='https://github.com/brookpatten/patrol.ski'> GitHub project</a>
                  </p>
              </CCol>
            </CRow>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
    <CRow>
      <CCol md="6" lg="6">
        <CCard>
          <CCardHeader>For Administrators</CCardHeader>
          <CCardBody>
              <ul>
                <li><p>Create your schedule quickly.  Schedule every shift individually or create "cookie cutter" shifts</p>
                  <img src="feature-schedule-calendar.png" alt="announcement screenshot" style="width:300px" @click="openGallery(12)"/>
                </li>
                <li><p>Allow your patrol to swap & pick up shifts, but you retain final approval</p>
                  <img src="feature-schedule-swap.png" alt="announcement screenshot" style="width:300px" @click="openGallery(15)"/>
                </li>
                <li><p>Quickly duplicate your schedule out of any period of time desired</p></li>
                <li><p>Create announcements and calendar events and communicate them to your patrol</p>
                  <img src="feature-announcement.png" alt="announcement screenshot" style="width:300px" @click="openGallery(0)"/>
                </li>
                <li><p>Assign training to members of your patrol and track their progress</p>
                  <img src="feature-assignments.png" alt="announcement screenshot" style="width:300px" @click="openGallery(2)"/>
                </li>
                <li><p>Manage multiple simultaneous training plans in a structured, consistent way</p>
                  <img src="feature-training-plans.png" alt="announcement screenshot" style="width:300px" @click="openGallery(16)"/>
                </li>
                <li><p>Choose from an existing library of training plans, customize them, or start from scratch.</p>
                  <img src="feature-edit-plan.png" alt="announcement screenshot" style="width:300px" @click="openGallery(6)"/>
                </li>
                <li><p>Fine grained control of which trainers can sign off on each requirement</p>
                  <img src="feature-edit-section.png" alt="announcement screenshot" style="width:300px" @click="openGallery(7)"/>
                </li>
                <li><p>Allow trainees to quickly find trainers that can help them by joining trainer credentials, training plans, and the schedule</p>
                  <img src="feature-available-training-shifts.png" alt="announcement screenshot" style="width:300px" @click="openGallery(3)"/>
                </li>
                <li><p>Manage training group sizes and help trainees find the trainers who can best help them.</p></li>
                <li><p>Use all of the above or just the parts you want.  Disable the parts you don't want</p>
                  <img src="feature-edit-patrol.png" alt="announcement screenshot" style="width:300px" @click="openGallery(5)"/>
                </li>
                <li><p>Track time, missed/late shifts from the built in timeclock (Coming soon)</p></li>
              </ul>
          </CCardBody>
        </CCard>
      </CCol>
      <CCol md="6" lg="6">
        <CCard>
          <CCardHeader>For Patrollers</CCardHeader>
          <CCardBody>
              <ul>
                <li><p>Easily access the schedule.</p>
                  <img src="feature-schedule-calendar.png" alt="announcement screenshot" style="width:300px" @click="openGallery(12)"/>
                </li>
                <li><p>Release shifts you need someone else to cover, Claim shifts you would like to cover.</p>
                  <img src="feature-schedule-swap.png" alt="announcement screenshot" style="width:300px" @click="openGallery(15)"/>
                </li>
                <li><p>Keep in touch with announcements and patrol events</p>
                  <img src="feature-announcement.png" alt="announcement screenshot" style="width:300px" @click="openGallery(0)"/>
                </li>
                <li><p>Stay aware of required training, frosties etc</p>
                  <img src="feature-my-assignments.png" alt="announcement screenshot" style="width:300px" @click="openGallery(9)"/>
                </li>
                <li><p>For Trainers, allow trainees to "sign up" to train with you on your shift.  Automatically notify them if your shift is cancelled or swapped.</p>
                </li>
              </ul>
          </CCardBody>
        </CCard>
      </CCol>
    </CRow>
    <CRow>
      <CCol md="6" lg="6">
        <CCard>
          <CCardHeader>For Trainees</CCardHeader>
          <CCardBody>
              <ul>
                <li><p>Easily check progress on training assignments</p>
                  <img src="feature-assignment.png" alt="announcement screenshot" style="width:300px" @click="openGallery(1)"/>
                </li>
                <li><p>Receive notifications when sign-offs are completed</p></li>
                <li><p>View upcoming shifts filtered by which trainers can help with your specific outstanding training requirements</p>
                  <img src="feature-available-training-shifts.png" alt="announcement screenshot" style="width:300px" @click="openGallery(3)"/>
                </li>
                <li><p>Sign up to train with trainers and let them know you're coming</p></li>
                <li><p>Receive a notification if a trainer releases a shift where you had planned to train with them</p></li>
              </ul>
          </CCardBody>
        </CCard>
      </CCol>
      <CCol md="6" lg="6">
        <CCard>
          <CCardHeader>For Everybody</CCardHeader>
          <CCardBody>
              <ul>
                <li><p>Receive email notifications for events that effect you directly</p></li>
                <li><p>Use the app with a PC, tablet, or mobile</p>
                  <img src="feature-mobile.png" alt="announcement screenshot" style="height:200px" @click="openGallery(20)"/>
                </li>
                <li><p>Log in with google, microsoft, or other account (Coming soon)</p></li>
                <li><p>Clock in/out of your shift (Coming soon)</p></li>
                <li><p>View "Who's On" to quickly see who is currently clocked in</p></li>
              </ul>
          </CCardBody>
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
