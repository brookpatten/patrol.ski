export default {
    //data () {
    //  return {
    //  }
    //},
    methods: {
        hasPermission: function(permission){
            return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        },
    },
    computed: {
      selectedPatrolId: function () {
        return this.$store.state.selectedPatrolId;
      },
      selectedPatrol: function (){
          return this.$store.getters.selectedPatrol;
      },
      user: function (){
          return this.$store.getters.user;
      }
    }
  }