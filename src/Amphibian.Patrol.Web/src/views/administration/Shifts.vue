<template>
    <div>
        <CCard id="shifts">
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-indent-increase"/> Shifts
            </slot>
            </CCardHeader>
            <CCardBody>
                <CAlert color="info">
                    <p>
                        Defining shifts allows you to schedule more quickly by pre-populating start & end times based on a shift selection.
                        When scheduling is done using a shift, the name of the shift will be shown on the schedule/calendar.  eg: "Morning Shift".
                    </p>
                    <p>
                        Defining shifts is not required to use scheduling, you may simply choose the beginning and end date/time of each individual schedule entry.
                        If you do choose to define shifts, you are not required to always schedule with defined shifts, you may still simply select a beginning and end time when scheduling.
                    </p>
                </CAlert>
                <CDataTable
                    striped
                    bordered
                    small
                    fixed
                    :items="shifts"
                    :fields="shiftFields"
                    sorter>
                    <template #buttons="data">
                    <td>
                        <CButtonGroup size="sm">
                        <CButton color="primary" :to="{ name: 'EditShift', params: { shiftId: data.item.id } }">Edit</CButton>
                        <!--<CButton color="danger" v-on:click="removeShift(data.item.id)">Remove</CButton>-->
                        </CButtonGroup>
                    </td>
                    </template>
                    <template #starts="data">
                    <td>{{data.item.startHour}}:{{(data.item.startMinute+"").padStart(2,"0")}}</td>
                    </template>
                    <template #ends="data">
                    <td>{{data.item.endHour}}:{{(data.item.endMinute+"").padStart(2,"0")}}</td>
                    </template>
                    <template #buttons-header>
                    <CButton color="primary" size="sm" :to="{name:'EditShift',params:{shiftId:null}}">New</CButton>
                    </template>
                </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>

export default {
  name: 'Shifts',
  components: {
  },
  data () {
    return {
      shifts: [],
      shiftFields:[
          {key:'name',label:'Name'},
          {key:'starts',label:'Shift Start'},
          {key:'ends',label:'Shift End'},
          {key:'buttons',label:'',sorter:false,filter:false}
      ]
    }
  },
  methods: {
    getShifts() {
      this.$store.dispatch('loading','Loading...');
        this.$http.get('schedule/shifts?patrolId='+this.selectedPatrolId)
            .then(response => {
                console.log(response);
                this.shifts = response.data;
            }).catch(response => {
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
    removeShift(id){
      this.$store.dispatch('loading','Removing...');
      this.$http.delete('schedule/shifts?shiftId='+id)
        .then(response=>{
          this.shifts = _.filter(this.shifts,function(x){return x.id!=id;});
        }).catch(response=>{
          console.log(response);
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    }
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    }
  },
  watch: {
    selectedPatrolId(){
      this.getShifts();
    }
  },
  mounted: function(){
      this.getShifts();
  }
}
</script>
