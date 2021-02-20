<template>
  <CHeader fixed with-subheader light>
    <CToggler
      in-header
      class="ml-3 d-lg-none"
      v-if="userId"
      @click="$store.commit('toggleSidebarMobile')"
    />
    <CToggler
      in-header
      class="ml-3 d-md-down-none"
      v-if="userId"
      @click="$store.commit('toggleSidebarDesktop')"
    />
    <CHeaderBrand class="mr-auto" to="/">
      <!--<CIcon name="logo" height="48" alt="Logo"/>-->
      <!--<template v-if="!selectedPatrol || !selectedPatrol.logoImageUrl">
        <img src="ms-icon-310x310.png" alt="patroller taking toboggan down slope" width="48" height="48"/>
      </template>--><span> </span>
    </CHeaderBrand>


    <CHeaderNav class="d-md-down-none mr-auto">
      <CHeaderNavItem class="px-3">
        <CHeaderNavLink v-if="userId && selectedPatrol" :to="{name:'Home'}">
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
      <TheHeaderDropdownAccnt v-if="userId"/>
      <CButtonGroup v-if="!userId">
      <CButton color="primary" :to="{name:'Login'}">Log In</CButton>
      </CButtonGroup>
    </CHeaderNav>
    <CSubheader class="px-3" v-if="userId">
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
    userId: function (){
      return this.$store.getters.userId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    }
  }
}
</script>
