<template>
    <div>
      <CRow><CCol>
      <CForm @submit.prevent="save">
      <CCard>
        <CCardHeader>
          <slot name="header">
              <CIcon name="cil-calendar"/> Repeat Schedule
          </slot>
        </CCardHeader>
        <CCardBody>
            <CAlert color="info">
              <p>This tool will copy a section of schedule from one date range into another date range, 1 day at a time.</p>
              <p>If the "to" range is longer than the "from" range, it will repeat the days from the "to" range</p>
              <p><strong>Example:</strong> to copy 1 day to the rest of the week, you could select "from" day one, "to" days 2,3,4,5</p>
              <p><strong>Example:</strong> to copy 1 week to the rest of the month, you could select "from" 11/29/20-12/5/20 "to" 12/6/20-1/2/21</p>
            </CAlert>
            <CAlert color="danger" v-if="validationMessage">{{validationMessage}}</CAlert>
            <CRow>
              <CCol>
                <label for="sourceStart">From Start</label>
                <datepicker v-model="sourceStart" input-class="form-control" calendar-class="card"></datepicker>
              </CCol>
              <CCol>
                <label for="sourceEnd">From End</label>
                <datepicker v-model="sourceEnd" input-class="form-control" calendar-class="card"></datepicker>
            </CCol>
            </CRow>
            <CRow>
              <CCol>
                <label for="targetStart">To Start</label>
                <datepicker v-model="targetStart" input-class="form-control" calendar-class="card"></datepicker>
              </CCol>
              <CCol>
                <label for="targetEnd">To End</label>
                <datepicker v-model="targetEnd" input-class="form-control" calendar-class="card"></datepicker>
              </CCol>
            </CRow>

            <CRow>
              <CCol>
                <label>Clear "To" Range First</label><br/>
                <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="clearTarget"/>
              </CCol>
              <CCol>
                <label>Test Only</label><br/>
                <CSwitch class="mx-1" color="primary" variant="3d" :checked.sync="testOnly"/>
              </CCol>
            </CRow>
        </CCardBody>
        <CCardFooter>
            <CButtonGroup>
              <CButton type="submit" color="primary">Copy</CButton>
            </CButtonGroup>
        </CCardFooter>
      </CCard>
      </CForm>
      </CCol></CRow>
      <CRow v-if="results && results.length>0">
        <CCol>
          <CCard>
            <CCardBody>
              <CDataTable
                    striped
                    bordered
                    small
                    fixed
                    :items="results"
                    :fields="resultsFields"
                    sorter>
                    <template #hours="data">
                        <td>
                          {{new Date(data.item.startsAt).toLocaleDateString()}}
                          {{new Date(data.item.startsAt).getHours()+":"+(new Date(data.item.startsAt).getMinutes()+"").padStart(2,"0")}}
                          -
                          {{new Date(data.item.endsAt).getHours()+":"+(new Date(data.item.endsAt).getMinutes()+"").padStart(2,"0")}}
                        </td>
                    </template>
                    <template #name="data">
                        <td>
                          <strong v-if="data.item.shift">{{data.item.shift.name}}</strong>
                           
                          <em v-if="data.item.group">{{data.item.group.name}}</em>
                        </td>
                    </template>
                </CDataTable>
            </CCardBody>
          </CCard>
        </CCol>
      </CRow>
    </div>
</template>

<script>

import Datepicker from 'vuejs-datepicker';

export default {
  name: 'RepeatSchedule',
  components: { Datepicker
  },
  props: [],
  data () {
    return {
      sourceStart:new Date((new Date()).getFullYear(),(new Date()).getMonth(),(new Date()).getDate()),
      sourceEnd:new Date((new Date()).getFullYear(),(new Date()).getMonth(),(new Date()).getDate()),
      targetStart: new Date((new Date()).getFullYear(),(new Date()).getMonth(),(new Date()).getDate()),
      targetEnd: new Date((new Date()).getFullYear(),(new Date()).getMonth(),(new Date()).getDate()),
      clearTarget:false,
      testOnly: true,
      results:[],
      resultsFields:[
          {key:'hours',label:'Date/Times',sortable:true},
          {key:'name',label:'',sortable:true}
      ],
      validationMessage:'',
      validationErrors:{},
      validated:false
    }
  },
  methods: {
    save(){
        this.$store.dispatch('loading','Repeating...');

        this.sourceStart.setHours(0,0,0,0);
        this.sourceEnd.setHours(23,59,59,999);
        this.targetStart.setHours(0,0,0,0);
        this.targetEnd.setHours(23,59,59,999);

        this.$http.post('schedule/replicate',{sourceStart:this.sourceStart,sourceEnd:this.sourceEnd,targetStart:this.targetStart,targetEnd:this.targetEnd,clearTarget:this.clearTarget,testOnly:this.testOnly, patrolId:this.selectedPatrolId})
          .then(response=>{
            this.results = response.data;
          }).catch(response=>{
            this.validated=true;
            this.validationMessage = response.response.data.title;
            this.validationErrors = response.response.data.errors;
          }).finally(response=>this.$store.dispatch('loadingComplete'));
    }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
  },
  watch: {
  },
  mounted: function(){
  }
}
</script>
