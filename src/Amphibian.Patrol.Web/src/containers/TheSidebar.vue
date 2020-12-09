<template>
  <CSidebar 
    unfoldable 
    :minimize="minimize"
    :show="show"
    @update:show="(value) => $store.commit('set', ['sidebarShow', value])"
  >
    <CSidebarBrand class="d-md-down-none" to="/">
      
    </CSidebarBrand>

    <!--<CRenderFunction flat :content-to-render="$options.nav"/>-->
    <CSidebarNav>
      <CSidebarNavItem name='Dashboard' :to='{name:"Home"}' icon='cil-speedometer'/>
      <CSidebarNavItem v-if="selectedPatrol.enableScheduling && !hasPermission('MaintainSchedule')"
        name='Calendar' :to='{name:"Calendar"}' icon='cil-calendar' />
      <CSidebarNavItem v-if="selectedPatrol.enableShiftSwaps && !hasPermission('MaintainSchedule')"
        name='Schedule Swap' :to='{name:"ScheduleSwap"}' icon='cil-calendar' />
      
      <template v-if="hasPermission('MaintainUsers') || hasPermission('MaintainGroups') || hasPermission('MaintainPatrol')">
        <CSidebarNavTitle>
          Administration
        </CSidebarNavTitle>
        <CSidebarNavItem v-if='hasPermission("MaintainPatrol")'
          name='Patrol' :to='{name:"EditPatrol"}' icon='cil-home' />
        <CSidebarNavItem v-if='hasPermission("MaintainUsers")'
          name='People' :to='{name:"People"}' icon='cil-user' />
        <CSidebarNavItem v-if='hasPermission("MaintainGroups")'
          name='Groups' :to='{name:"Groups"}' icon='cil-people' />
      </template>

      <template v-if="(selectedPatrol.enableScheduling && hasPermission('MaintainSchedule'))">
        <CSidebarNavTitle>
          Schedule
        </CSidebarNavTitle>
        <CSidebarNavItem name='Calendar' :to='{name:"Calendar"}' icon='cil-calendar' />
        <CSidebarNavItem v-if="selectedPatrol.enableShiftSwaps" name='Schedule Swap' :to='{name:"ScheduleSwap"}' icon='cil-list' />
        <CSidebarNavItem v-if="selectedPatrol.enableShiftSwaps" name='Swap Approval' :to='{name:"SwapApproval"}' icon='cil-list' />
        <CSidebarNavItem name='Shifts' :to='{name:"Shifts"}' icon='cil-indent-increase' />
      </template>

      <template v-if="(selectedPatrol.enableTimeClock && hasPermission('MaintainTimeClock'))">
        <CSidebarNavTitle>
          Timeclock
        </CSidebarNavTitle>
        <CSidebarNavItem name='Time/Days' :to='{name:"TimeDays"}' icon='cil-calendar' />
        <template v-if="selectedPatrol.enableScheduling">
          <CSidebarNavItem name='Time Missing' :to='{name:"TimeMissing"}' icon='cil-calendar' />
        </template>
        <CSidebarNavItem name='Clock In/Out' :to='{name:"TimeEntries"}' icon='cil-calendar' />
      </template>

      <template v-if="(selectedPatrol.enableAnnouncements && hasPermission('MaintainAnnouncements')) || (selectedPatrol.enableEvents && hasPermission('MaintainEvents'))">
        <CSidebarNavTitle>
          Communication
        </CSidebarNavTitle>
        <CSidebarNavItem v-if='selectedPatrol.enableAnnouncements && hasPermission("MaintainAnnouncements")'
          name='Announcements' :to='{name:"Announcements"}' icon='cil-comment-square' />
        <CSidebarNavItem v-if='selectedPatrol.enableEvents && hasPermission("MaintainEvents")'
          name='Events' :to='{name:"Events"}' icon='cil-calendar' />
      </template>
      
      <template v-if="selectedPatrol.enableTraining && (hasPermission('MaintainPlans') || hasPermission('MaintainAssignments'))">
        <CSidebarNavTitle>
          Training
        </CSidebarNavTitle>
        <CSidebarNavItem v-if='selectedPatrol.enableTraining && hasPermission("MaintainPlans")'
          name='Plans' :to='{name:"Plans"}' icon='cil-spreadsheet' />
        <CSidebarNavItem v-if='selectedPatrol.enableTraining && hasPermission("MaintainAssignments")'
          name='Assignments' :to='{name:"Assignments"}' icon='cil-list-rich' />
      </template>

      
      
    </CSidebarNav>
      
    <CSidebarMinimizer
      class="d-md-down-none"
      @click.native="$store.commit('set', ['sidebarMinimize', !minimize])"
    />
  </CSidebar>
</template>

<script>
import nav from './_nav'
import { freeSet } from '@coreui/icons'

export default {
  name: 'TheSidebar',
  nav,
  computed: {
    show () {
      return this.$store.state.sidebarShow 
    },
    minimize () {
      return this.$store.state.sidebarMinimize 
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    },
    showAdministration: function(){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && this.selectedPatrol.permissions.length>0;
    }
  },
  methods: {
    hasPermission: function(permission){
      return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
    }
  }
}
</script>
