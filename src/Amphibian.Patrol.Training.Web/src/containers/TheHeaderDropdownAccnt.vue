<template>
  <CDropdown
    inNav
    class="c-header-nav-items"
    placement="bottom-end"
    add-menu-classes="pt-0"
  >
    <template #toggler>
      <CHeaderNavLink>
        <div class="c-avatar">
          <img
            src="img/avatars/7.jpg"
            class="c-avatar-img "
          />
        </div>
      </CHeaderNavLink>
    </template>
    <CDropdownHeader tag="div" class="text-center" color="light">
      <strong>Account</strong>
    </CDropdownHeader>
    <CDropdownItem>
      <CIcon name="cil-user" /> Profile
    </CDropdownItem>
    <CDropdownItem>
      <CIcon name="cil-lock-locked" /> Logout
    </CDropdownItem>
    <CDropdownHeader tag="div" class="text-center" color="light" v-if="patrols && patrols.length>1">
      <strong>Patrols</strong>
    </CDropdownHeader>
    <CDropdownItem v-for="patrol in patrols" :key="patrol.id" v-on:click="selectPatrol(patrol.id)">
      <CIcon name="cil-check-circle" v-if="patrol.id===selectedPatrolId"/><CIcon name="cil-circle" v-if="patrol.id!=selectedPatrolId"/> {{patrol.name}}
    </CDropdownItem>
    <CDropdownHeader
      tag="div"
      class="text-center"
      color="light"
    >
      <strong>Settings</strong>
    </CDropdownHeader>
    
    <CDropdownItem>
      <CIcon name="cil-settings" /> Settings
    </CDropdownItem>
  </CDropdown>
</template>

<script>
export default {
  name: 'TheHeaderDropdownAccnt',
  data () {
    return { 
      itemsCount: 42
    }
  },
  computed: {
    patrols: function () {
      return this.$store.state.patrols;
    },
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    }
  },
  methods: {
    selectPatrol: function(id){
      this.$store.dispatch('change_patrol',id)
        .then(()=>this.$router.push(''))
        .catch(err => console.log(err));
    }
  }
}
</script>

<style scoped>
  .c-icon {
    margin-right: 0.3rem;
  }
</style>