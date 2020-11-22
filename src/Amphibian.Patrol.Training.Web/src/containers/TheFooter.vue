<template>
  <CFooter :fixed="false">
    <div>
      <a href="https://Patrol.Ski" target="_blank">Patrol.Ski</a>
      <span class="ml-1">&copy; {{new Date().getFullYear()}} Amphibian Software</span>
    </div>
    <div class="ml-auto">
      <CButton color="link" :to="{name:'Help'}">Need Help?</CButton>
      <span class="mr-1">Visit this App on </span>
      <a href="https://github.com/brookpatten/patrol.ski" target="_blank">GitHub</a>
    </div>
  </CFooter>
</template>

<script>
export default {
  name: 'TheFooter'
}
</script>
