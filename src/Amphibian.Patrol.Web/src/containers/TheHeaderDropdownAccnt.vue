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
            src="img/avatars/patrol-50.jpg"
            class="c-avatar-img "
          />
        </div>
      </CHeaderNavLink>
    </template>
    <CDropdownHeader tag="div" class="text-center" color="light" v-if="user">
      <strong>{{user.firstName}} {{user.lastName}}</strong>
    </CDropdownHeader>
    <CDropdownItem :to="{name:'Profile'}">
      <CIcon name="cil-user" /> Profile
    </CDropdownItem>
    <CDropdownItem v-on:click="logout">
      <CIcon name="cil-lock-locked" /> Logout
    </CDropdownItem>
    <template v-if="patrols && patrols.length>1">
      <CDropdownHeader tag="div" class="text-center" color="light">
        <strong>Patrols</strong>
      </CDropdownHeader>
      <CDropdownItem v-for="patrol in patrols" :key="patrol.id" v-on:click="selectPatrol(patrol.id)">
        <CIcon name="cil-check-circle" v-if="patrol.id===selectedPatrolId"/><CIcon name="cil-circle" v-if="patrol.id!=selectedPatrolId"/> {{patrol.name}}
      </CDropdownItem>
    </template>
    <CDropdownItem :to="{name:'NewPatrol'}">
      <CIcon name="cil-user-follow" /> New Patrol
    </CDropdownItem>
    <!--<CDropdownHeader
      tag="div"
      class="text-center"
      color="light"
    >
      <strong>Settings</strong>
    </CDropdownHeader>
    
    <CDropdownItem>
      <CIcon name="cil-settings" /> Settings
    </CDropdownItem>-->
  </CDropdown>
</template>

<script>
export default {
  name: 'TheHeaderDropdownAccnt',
  data () {
    return { 
      user:{}
    }
  },
  computed: {
    patrols: function () {
      return this.$store.state.patrols;
    },
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    userId: function (){
        return this.$store.getters.userId;
    }
  },
  methods: {
    selectPatrol: function(id){
      this.$store.dispatch('change_patrol',id)
        .then(()=>this.$router.push(''))
        .catch(err => console.log(err));
    },
    logout: function(){
      this.$store.dispatch('logout')
        .then(()=>this.$router.push({name:"Login"}))
        .catch(err => console.log(err));
    },
    getUser:function(){
      this.$http.get('user').then(resp=>{
        this.user = resp.data;
      });
    }
  },
  watch: {
    userId(){
        if(this.userId!=0){
          this.getUser();
        }
    }
  },
  mounted: function(){
    this.getUser();
  }
}
</script>

<style scoped>
  .c-icon {
    margin-right: 0.3rem;
  }
</style>