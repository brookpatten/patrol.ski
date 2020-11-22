<template>
  <CFooter :fixed="false">
    <div>
      <a href="https://Patrol.Training" target="_blank">Patrol.Training</a>
      <span class="ml-1">&copy; {{new Date().getFullYear()}} Amphibian Software</span>
    </div>
    <div class="ml-auto">
      <span class="mr-1">Powered by</span>
      <a href="https://coreui.io/vue" target="_blank">CoreUI for Vue</a>
    </div>
  </CFooter>
</template>

<script>
export default {
  name: 'TheFooter'
}
</script>
