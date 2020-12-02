<template>
  <CHeader fixed with-subheader light>
    <CToggler
      in-header
      class="ml-3 d-lg-none"
      v-if="user"
      @click="$store.commit('toggleSidebarMobile')"
    />
    <CToggler
      in-header
      class="ml-3 d-md-down-none"
      v-if="user"
      @click="$store.commit('toggleSidebarDesktop')"
    />
    <CHeaderBrand class="mr-auto" to="/">
      <!--<CIcon name="logo" height="48" alt="Logo"/>-->
      <img src="ms-icon-310x310.png" alt="patroller taking toboggan down slope" width="48" height="48"/>
    </CHeaderBrand>


    <CHeaderNav class="d-md-down-none mr-auto">
      <CHeaderNavItem class="px-3">
        <CHeaderNavLink v-if="user && selectedPatrol" :to="{name:'Home'}">
          {{selectedPatrol.name}}
        </CHeaderNavLink>
      </CHeaderNavItem>
    </CHeaderNav>
    
    
    <CHeaderNav class="mr-4">
      <!--<CHeaderNavItem class="d-md-down-none mx-2">
        <CHeaderNavLink>
          <CIcon name="cil-bell"/>
        </CHeaderNavLink>
      </CHeaderNavItem>-->
      <TheHeaderDropdownAccnt v-if="user"/>
      <CButtonGroup v-if="!user">
      <CButton color="primary" :to="{name:'Login'}">Log In</CButton>
      </CButtonGroup>
    </CHeaderNav>
    <CSubheader class="px-3" v-if="user">
      <CBreadcrumbRouter class="border-0 mb-0"/>
    </CSubheader>
  </CHeader>
</template>

<script>
import TheHeaderDropdownAccnt from './TheHeaderDropdownAccnt'

export default {
  name: 'TheHeader',
  components: {
    TheHeaderDropdownAccnt
  },
  computed:{
    user: function (){
      if(this.$store.getters.user && this.$store.getters.user.id){
        return this.$store.getters.user;
      }
      else{
        return null;
      }
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    }
  }
}
</script>
