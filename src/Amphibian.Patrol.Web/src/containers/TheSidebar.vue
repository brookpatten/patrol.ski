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
      
      <CSidebarNavTitle v-if="hasPermission('MaintainUsers') || hasPermission('MaintainGroups') || hasPermission('MaintainPatrol')">
        Administration
      </CSidebarNavTitle>
      <CSidebarNavItem v-if='hasPermission("MaintainPatrol")'
        name='Patrol' :to='{name:"EditPatrol"}' icon='cil-home' />
      <CSidebarNavItem v-if='hasPermission("MaintainUsers")'
        name='People' :to='{name:"People"}' icon='cil-user' />
      <CSidebarNavItem v-if='hasPermission("MaintainGroups")'
        name='Groups' :to='{name:"Groups"}' icon='cil-people' />
      
      <CSidebarNavTitle v-if="selectedPatrol.enableTraining && (hasPermission('MaintainPlans') || hasPermission('MaintainAssignments'))">
        Training
      </CSidebarNavTitle>
      <CSidebarNavItem v-if='selectedPatrol.enableTraining && hasPermission("MaintainPlans")'
        name='Plans' :to='{name:"Plans"}' icon='cil-spreadsheet' />
      <CSidebarNavItem v-if='selectedPatrol.enableTraining && hasPermission("MaintainAssignments")'
        name='Assignments' :to='{name:"Assignments"}' icon='cil-pencil' />

      <CSidebarNavTitle v-if="selectedPatrol.enableAnnouncements && (hasPermission('MaintainAnnouncements') || hasPermission('MaintainEvents'))">
        Communication
      </CSidebarNavTitle>
      <CSidebarNavItem v-if='selectedPatrol.enableAnnouncements && hasPermission("MaintainAnnouncements")'
        name='Announcements' :to='{name:"Announcements"}' icon='cil-comment-square' />
      <!--<CSidebarNavItem v-if='selectedPatrol.enableAnnouncements && hasPermission("MaintainEvents")'
        name='Events' :to='{name:"Events"}' icon='cil-pencil' />-->
      
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
