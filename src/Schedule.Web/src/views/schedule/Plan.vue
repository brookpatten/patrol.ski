<template>
    <div>
        <CCard>
            <CCardHeader>
            <slot name="header">
                <CIcon name="cil-grid"/> {{caption}}
            </slot>
            </CCardHeader>
            <CCardBody>
            <CDataTable
                :hover="hover"
                :striped="true"
                :bordered="true"
                :small="small"
                :fixed="fixed"
                :items="demo"
                :fields="demoFields"
                :items-per-page="small ? 10 : 5"
                :dark="dark"
                pagination
            >
                <template #status="{item}">
                <td>
                    <CBadge :color="getBadge(item.status)">{{item.status}}</CBadge>
                </td>
                </template>
            </CDataTable>
            </CCardBody>
        </CCard>
    </div>
</template>

<script>

export default {
  name: 'Plan',
  components: {
  },
  data () {
    return {
      plan: {
          skills: [
              {id:1, name: 'Gliding Wedge'},
              {id:2, name: 'Hockey Stop'},
          ],
          levels: [
              {id:1, name:'1'},
              {id:2, name:'2'}
          ],
          signatures: [],
      },
      demo: [
          {name:'Hockey Stop',lvl1:'',lvl2a:'',lvl2b:'',final:''},
          {name:'Gliding Wedge',lvl1:'',lvl2a:'',lvl2b:'',final:''}
      ],
      demoFields:[
          {key:'name', label:'Skill', stickyColumn:true},
          {key:'lvl1', label:'Level 1'},
          {key:'lvl2a', label:'Level 2+'},
          {key:'lvl2b', label:'Level 2+'},
          {key:'final', label:'Final'}
      ]
    }
  },
  methods: {
    
  }
}
</script>
