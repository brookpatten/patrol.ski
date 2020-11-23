<template>
  <CContainer class="d-flex content-center min-vh-100">
  </CContainer>
</template>

<script>
export default {
  name: 'TestDrive',
  data(){
      return {
      }
    },
  methods: {
    throwaway: function(){
      this.dispatchLogin({throwaway:true});
    },
    dispatchLogin:function(obj){
      this.$store.dispatch('loading','Creating Fictional Patrol...');
      this.$store.dispatch('login',obj)
        .then(()=>{
          //if they have any patrols go there, otherwise create a new patrol
          this.$router.push({name:'Home'});
        })
        .catch(err => {
          console.log(err);
          this.error = "Something didn't work";
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    }
  },
  mounted(){
      this.throwaway();
  }
}
</script>
