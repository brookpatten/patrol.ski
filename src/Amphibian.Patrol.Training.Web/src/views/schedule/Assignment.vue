<template>
    <div>
        <CCard>
            <CCardHeader>{{user.firstName}} {{user.lastName}} - {{plan.name}}</CCardHeader>
            <CCardBody v-if="signOffTable.length>0">
                <table class="table table-responsive table-bordered">
                    <thead class="thead-dark">
                        <tr>
                            <th :colspan="levels.length+1">
                                <template v-if="assignment.assignedAt">Assigned: {{(new Date(assignment.assignedAt)).toLocaleDateString()}}<br/></template>
                                <template v-if="assignment.completedAt">Completed: {{(new Date(assignment.completedAt)).toLocaleDateString()}}<br/></template>
                                <template v-if="!assignment.completedAt && assignment.dueAt">Due: {{(new Date(assignment.dueAt)).toLocaleDateString()}}</template> 
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <template v-for="signOffRow in signOffTable">
                            <tr v-if="signOffRow.header" :key="signOffRow.index+'-header-row'">
                                <td></td>
                                <th v-for="section in signOffRow.header.sections" :key="section.columnIndex+'-'+section.rowIndex+'-header'" :colspan="section.columnCount">
                                <span>{{section.name}}</span>
                                </th>
                            </tr>
                            <tr v-if="signOffRow.header" :key="signOffRow.index+'-level-row'">
                                <td>Skill</td>
                                <td v-for="level in levels" :key="level.id">{{level.level.name}}</td>
                            </tr>
                            <tr :key="signOffRow.index+'skill-row'">
                                <td>{{signOffRow.skill.name}}</td>
                                <td v-for="signOff in signOffRow.signOffs" :key="signOff.id" v-bind:style="{ backgroundColor: signOff.section ? signOff.section.color : null, color:'#000000'}">
                                    <span v-if="signOff!=null && signOff.signature!=null">
                                        <CIcon :content="$options.freeSet.cilTask"/>
                                        {{signOff.signature.signedBy.firstName}} {{signOff.signature.signedBy.lastName}} 
                                        {{new Date(signOff.signature.signedAt).toLocaleDateString()}}
                                    </span>
                                    <span v-if="signOff!=null && signOff.signature==null && !signOff.currentUserCanSign"><CIcon :content="$options.freeSet.cilSquare"/></span>
                                    <span v-if="signOff!=null && signOff.signature==null && signOff.currentUserCanSign">
                                        <input type="checkbox" :value="{sectionLevelId:signOff.sectionlevel.id,sectionSkillId: signOff.sectionSkill.id}" name="signOff.id" v-model="newSignatures"/>
                                    </span>
                                </td>
                            </tr>
                        </template>
                    </tbody>
                </table>
            </CCardBody>
            <CCardFooter v-if="newSignatures.length>0 || hasPermission('MaintainAssignments')">
                <CButtonGroup class="float-right">
                    <CButton v-if="newSignatures.length>0" v-on:click="sign()" color="primary"><CIcon :content="$options.freeSet.cilPen"/>&nbsp;Sign</CButton>
                    <CButton v-if="hasPermission('MaintainAssignments')" :to="{name:'EditAssignment',params:{assignmentId:assignmentId}}" color="success">Edit Assignment</CButton>
                </CButtonGroup>
            </CCardFooter>
        </CCard>
    </div>
</template>

<script>
import { freeSet } from '@coreui/icons'
export default {
  name: 'Assignment',
  freeSet,
  components: {
  },
  props: ['assignmentId'],
  data () {
    return {
        assignment: {},
        plan:{},
        user:{},
        sortedSections:[],
        signOffTable:[],
        levels:[],
        newSignatures:[]
    }
  },
  methods: {
        getAssignment(assignmentId) {
            this.$store.dispatch('loading','Loading...');
            this.$http.get('assignment/'+assignmentId)
                .then(response => {
                    console.log(response);
                    this.assignment = response.data.assignment;
                    this.plan = response.data.plan;
                    this.user = response.data.user;
                    this.tabelize();
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        sign(){
            this.$store.dispatch('loading','Saving...');
            this.$http.post('assignment/sign',{assignmentId:this.assignment.id,signatures:this.newSignatures})
                .then(response => {
                    console.log(response);
                    this.assignment.signatures = response.data;
                    this.newSignatures = [];
                    this.tabelize();
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        last(arr){
          return _.last(arr);
        },
        sortSections(){
            //figure out the size and position of each section
            for(var i=0;i<this.plan.sections.length;i++){
                this.plan.sections[i].rowIndex = _.min(_.map(this.plan.sections[i].skills,x=>x.rowIndex));
                this.plan.sections[i].rowCount = this.plan.sections[i].skills.length;
                this.plan.sections[i].columnIndex = _.min(_.map(this.plan.sections[i].levels,x=>x.columnIndex));
                this.plan.sections[i].columnCount = this.plan.sections[i].levels.length;
            }

            //sort by row, then by column
            this.sortedSections = _.orderBy(this.plan.sections,['rowIndex','columnIndex']);
            this.plan.sections = this.sortedSections;
        },
        tabelize(){
            //convert the list of sections into a 2 dimensional array for easy html formatting
            this.signOffTable=[];
            this.levels=[];

            this.sortSections();
            
            for(var i=0;i<this.sortedSections.length;i++){
                for(var s=this.sortedSections[i].rowIndex;s<this.sortedSections[i].rowIndex + this.sortedSections[i].rowCount;s++){
                    while(this.signOffTable.length<=s){
                        this.signOffTable.push({skill:null,signOffs:[]});
                    }

                    var sectionSkill = _.find(this.sortedSections[i].skills,{rowIndex:s});
                    if(this.signOffTable[s].skill == null){
                        this.signOffTable[s].skill = sectionSkill.skill;
                    }

                    //if we're in the first row of a section, make sure to set up a header row
                    if(s==this.sortedSections[i].rowIndex){
                      if(this.signOffTable[s].header == null) {
                          this.signOffTable[s].header = {sections:[]};
                      }
                      this.signOffTable[s].header.sections.push(this.sortedSections[i]);
                      this.signOffTable[s].header.sections = _.orderBy(this.signOffTable[s].header.sections,['columnIndex']);
                    }

                    for(var l=this.sortedSections[i].columnIndex;l<this.sortedSections[i].columnIndex + this.sortedSections[i].columnCount;l++){
                        var sectionLevel = _.find(this.sortedSections[i].levels,{columnIndex:l});

                        while(this.levels.length<=l){
                            this.levels.push(null);
                        }
                        this.levels[l] = sectionLevel;
                        
                        while(this.signOffTable[s].signOffs.length<=l){
                            this.signOffTable[s].signOffs.push({id:sectionLevel.id+'-'+sectionSkill.id});
                        }

                        var signature = _.find(this.assignment.signatures,{sectionSkillId:sectionSkill.id,sectionLevelId:sectionLevel.id});
                        
                        this.signOffTable[s].signOffs[l].columnIndex= l;
                        this.signOffTable[s].signOffs[l].rowIndex= s;
                        this.signOffTable[s].signOffs[l].sectionlevel= sectionLevel;
                        this.signOffTable[s].signOffs[l].sectionSkill= sectionSkill;
                        this.signOffTable[s].signOffs[l].section = this.sortedSections[i];
                        this.signOffTable[s].signOffs[l].signature= signature;
                        this.signOffTable[s].signOffs[l].currentUserCanSign = this.sortedSections[i].currentUserCanSign;
                        
                    }
                }
            }

            //pad out rows/columns to make things uniform
            for(var r=0;r<this.signOffTable.length;r++){
              this.signOffTable[r].index = r;

              //pad out header
              if(this.signOffTable[r].header!=null){
                for(var h=0;h<this.signOffTable[r].header.sections.length;h++){
                  var current = this.signOffTable[r].header.sections[h];
                  var previous = null;
                  if(h>0){
                    previous = this.signOffTable[r].header.sections[h-1]
                  }
                  var previousColumn = previous ? previous.columnIndex + previous.columnCount : 0;
                  if(previousColumn!=current.columnIndex){
                    var blank = { columnCount: current.columnIndex - previousColumn, columnIndex:previousColumn };
                    this.signOffTable[r].header.sections.push(blank);
                    this.signOffTable[r].header.sections = _.orderBy(this.signOffTable[r].header.sections,['columnIndex']);
                    h++;
                  }

                  if(h+1<this.signOffTable[r].header.sections.length){
                      var next = this.signOffTable[r].header.sections[h+1];

                      if(next.columnIndex - (current.columnIndex + current.columnCount)>1){
                        var blank = { columnCount: next.columnIndex - (current.columnIndex + current.columnCount), columnIndex:current.columnIndex+current.columnCount };
                        this.signOffTable[r].header.sections.push(blank);
                        this.signOffTable[r].header.sections = _.orderBy(this.signOffTable[r].header.sections,['columnIndex']);
                        h++;
                      }
                  }
                  else if(h+1==this.signOffTable[r].header.sections.length && current.columnIndex + current.columnCount < this.levels.length ){
                    var blank = { columnCount: this.levels.length - (current.columnIndex + current.columnCount), columnIndex:current.columnIndex+current.columnCount };
                    this.signOffTable[r].header.sections.push(blank);
                    this.signOffTable[r].header.sections = _.orderBy(this.signOffTable[r].header.sections,['columnIndex']);
                    h++;
                  }
                }
              }

              //pad out cells
              while(this.signOffTable[r].signOffs.length<this.levels.length){
                  this.signOffTable[r].signOffs.push({rowIndex:r,columnIndex:_.last(this.signOffTable[r].signOffs).columnIndex+1});
              }
            }

            //index levels
            for(var i=0;i<this.levels.length;i++){
              this.levels[i].index=i;
            }
        },
        hasPermission: function(permission){
          return this.selectedPatrol!=null && this.selectedPatrol.permissions!=null && _.indexOf(this.selectedPatrol.permissions,permission) >= 0;
        }
  },
  mounted: function(){
      this.getAssignment(this.assignmentId);
  },
  computed: {
    selectedPatrolId: function () {
      return this.$store.state.selectedPatrolId;
    },
    selectedPatrol: function (){
        return this.$store.getters.selectedPatrol;
    }
  }
}
</script>
