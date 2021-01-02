<template>
    <CModal
        :show.sync="show"
        :no-close-on-backdrop="true"
        color="primary"
        size="lg"
        >
    <CRow>
        
    </CRow>
    </CModal>
</template>

<script>

import AuthenticatedPage from '../../mixins/AuthenticatedPage';

export default {
  name: 'WorkItemDetailsModal',
  mixins: [AuthenticatedPage],
  components: {
  },
  props:[workItemId],
  data () {
    return {
      show:false,
      workItem: {}
    }
  },
  methods: {
    getWorkItem() {
      this.$store.dispatch('loading','Loading...');
      this.$http.get('workitem/'+this.workItemId)
        .then(response => {
            console.log(response);
            this.workItem = response.data;
            this.show=true;
        }).catch(response => {
            console.log(response);
        }).finally(response=>this.$store.dispatch('loadingComplete'));
    },
    refresh(){
      if(this.workItemId){
        this.getWorkItem();
      }
    }
  },
  computed: {
    
  },
  watch: {
    selectedPatrolId(){
      this.refresh();
    },
    workItemId(){
      this.refresh();
    }
  },
  mounted: function(){
    this.refresh();
  },
  beforeDestroy: function(){
  }
}
</script>
