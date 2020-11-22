<template>
  <CSidebar 
    unfoldable 
    :minimize="minimize"
    :show="show"
    @update:show="(value) => $store.commit('set', ['sidebarShow', value])"
  >
    <CSidebarBrand class="d-md-down-none" to="/">
      <h3 v-if="selectedPatrol==null">Training</h3>
      <h3 v-if="selectedPatrol">{{selectedPatrol.name}}</h3>
    </CSidebarBrand>

    <!--<CRenderFunction flat :content-to-render="$options.nav"/>-->
    <CSidebarNav>
      <CSidebarNavItem name='Dashboard' to='/home' icon='cil-speedometer'/>
      <!--<CSidebarNavItem name='Plan'  to='/plan'  icon='cil-grid'/>
      <CSidebarNavItem name='Schedule' to='/schedule' icon='cil-calendar'/>-->
      <CSidebarNavTitle v-if="showAdministration">
        Administration
      </CSidebarNavTitle>
      <CSidebarNavItem v-if='hasPermission("MaintainUsers")'
        name='People' to='/administration/people' icon='cil-people' />
      <CSidebarNavItem v-if='hasPermission("MaintainGroups")'
        name='Groups' to='/administration/groups' icon='cil-chart-pie' />
      <!--<CSidebarNavItem v-if='hasPermission("MaintainPatrol")'
        name='Integration' to='/administration/integration' icon='cil-cloud-download' />-->
      <CSidebarNavItem v-if='hasPermission("MaintainPlans")'
        name='Training Plans' to='/administration/plans' icon='cil-grid' />
      <CSidebarNavItem v-if='hasPermission("MaintainAssignments")'
        name='Assignments' to='/administration/assignments' icon='cil-pencil' />
      <!--<CSidebarNavItem v-if='hasPermission("MaintainPlans")'
        name='Skills' to='/administration/skills' icon='cil-list' />
      <CSidebarNavItem v-if='hasPermission("MaintainPlans")'
        name='Levels' to='/administration/levels' icon='cil-layers' />-->
    </CSidebarNav>
      
    <CSidebarMinimizer
      class="d-md-down-none"
      @click.native="$store.commit('set', ['sidebarMinimize', !minimize])"
    />
  </CSidebar>
</template>

<script>
import nav from './_nav'

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
