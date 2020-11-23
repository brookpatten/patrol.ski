<template>
    <div>
        <CCard v-if="editor=='plan'">
            <CCardHeader>
              <div v-if="!edit">{{plan.name}}</div>
              <CInput v-if="edit" v-model="plan.name"></CInput>
            </CCardHeader>
            <CCardBody v-if="signOffTable.length>0">
                <table class="table table-responsive table-bordered">
                    <thead class="thead-dark">
                        <tr>
                            <th scope="col">Skill</th>
                            <th scope="col" v-for="level in levels" :key="level ? level.index+'-level' : 'blank-level'">
                              <template v-if="level">
                                <div>{{level.level.name}}</div>
                                
                                <CButtonGroup v-if="edit">
                                  <CButton v-on:click="beginEditLevel(level)" size="sm" color="info"><CIcon :content="$options.freeSet.cilPencil"/></CButton>
                                  <CButton v-if="allowRemoveLevel" size="sm" color="danger" v-on:click="removeLevel(level)"><CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                                  <CButton v-if="last(levels)==level" size="sm" color="success">Level <CIcon :content="$options.freeSet.cilExpandRight"/></CButton>
                                  <CButton v-if="last(levels)==level" size="sm" color="primary" v-on:click="addSection(0,signOffTable.length,levels.length,1)">Section <CIcon :content="$options.freeSet.cilExpandRight"/></CButton>
                                </CButtonGroup>
                              </template>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <template v-for="signOffRow in signOffTable">
                          <tr v-if="signOffRow.header" :key="signOffRow.index+'-header-row'">
                            <td></td>
                            <th v-for="section in signOffRow.header.sections" :key="section.columnIndex+'-'+section.rowIndex+'-header'" :colspan="section.columnCount">
                              <div>{{section.name}}</div>
                              <CButtonGroup v-if="edit">
                                <CButton v-if="section.id || section.id==0" v-on:click="beginEditSection(section)" size="sm" color="info"><CIcon :content="$options.freeSet.cilPencil"/></CButton>
                                <CButton v-if="(section.id || section.id==0) && allowRemoveSection" v-on:click="removeSection(section)" size="sm" color="danger"><CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                                <CButton v-if="(section.id || section.id==0) && last(signOffRow.header.sections)==section" size="sm" color="success">Level <CIcon :content="$options.freeSet.cilExpandRight"/></CButton>
                                <CButton v-if="(section.id || section.id==0) && last(signOffRow.header.sections)==section" size="sm" color="primary" v-on:click="addSection(section.rowIndex,null,section.columnIndex + section.columnCount,1,section)">Section <CIcon :content="$options.freeSet.cilExpandRight"/></CButton>
                              </CButtonGroup>
                            </th>
                          </tr>
                          <tr :key="signOffRow.index+'-skill-row'">
                            <td>
                              <div>{{signOffRow.skill.name}}</div>
                              <CButtonGroup>
                                <CButton v-if="edit" v-on:click="beginEditSkill(signOffRow)" size="sm" color="info"><CIcon :content="$options.freeSet.cilPencil"/></CButton>
                                <CButton v-if="edit && allowRemoveSkill" v-on:click="removeSkill(signOffRow.skill)" size="sm" color="danger"><CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                                <CButton v-if="edit" size="sm" color="success">Skill <CIcon :content="$options.freeSet.cilExpandDown"/></CButton>
                                <CButton v-if="edit && last(signOffTable)==signOffRow" size="sm" color="primary" v-on:click="addSection(signOffRow.index+1,1,null,null,null)">Section <CIcon :content="$options.freeSet.cilExpandDown"/></CButton>
                              </CButtonGroup>
                            </td>
                            <td v-for="signOff in signOffRow.signOffs" :key="signOff.rowIndex+'-'+signOff.columnIndex+'-skill-buttons'" v-bind:style="{ backgroundColor: signOff.section ? signOff.section.color : null}">
                                <div v-if="signOff!=null && signOff.section"><input type="checkbox" disabled/></div>
                                <CButtonGroup v-if="edit && signOffRow.footer && signOff !=null && signOff.section != null && signOff.columnIndex == signOff.section.columnIndex + signOff.section.columnCount -1 && signOff.rowIndex == signOff.section.rowIndex + signOff.section.rowCount -1">
                                  <CButton v-if="allowRemoveSkill" v-on:click="removeSkill(signOffRow.skill,signOff.section)" size="sm" color="danger" class="float-right"><CIcon :content="$options.freeSet.cilXCircle"/></CButton>
                                  <CButton v-if="edit && last(signOffTable)==signOffRow" size="sm" color="success">Skill <CIcon :content="$options.freeSet.cilExpandDown"/></CButton>
                                  <CButton v-if="edit && last(signOffTable)==signOffRow" size="sm" color="primary" v-on:click="addSection(signOff.section.rowIndex + signOff.section.rowCount,1,signOff.section.columnIndex,null,signOff.section)">Section <CIcon :content="$options.freeSet.cilExpandDown"/></CButton>
                                </CButtonGroup>
                            </td>
                          </tr>
                          
                        </template>
                    </tbody>
                </table>
            </CCardBody>
            <CCardFooter>
              <CButtonGroup class="float-right">
                <CButton v-if="!edit" v-on:click="edit=true" color="success">Edit <CIcon :content="$options.freeSet.cilPencil"/></CButton>
                <CButton v-if="edit" v-on:click="edit=false" color="success">Preview <CIcon :content="$options.freeSet.cilMagnifyingGlass"/></CButton>
                <CButton color="primary" v-on:click="save">Save <CIcon :content="$options.freeSet.cilSave"/></CButton>
              </CButtonGroup>
            </CCardFooter>
        </CCard>
        <CCard v-if="editor=='section'">
          <CCardHeader>Edit Section</CCardHeader>
          <CCardBody>
            <CRow>
              <CCol md="3">
                <label>Name</label>
                <CInput v-model="editSection.name"></CInput>
              </CCol>
              <CCol md="3">
                <label>Color</label><br/>
                <v-swatches v-model="editSection.color" show-fallback fallback-input-type="color" popover-x="right"></v-swatches>
              </CCol>
            </CRow>
            <CRow>
              <CCol>
                <label>Group(s) who can Sign</label>
                <div v-for="group in allGroups" :key="group.id">
                    <input type="checkbox" :value="group.id" v-model="group.selected"/> <label>{{group.name}}</label>
                </div>
              </CCol>
            </CRow>
          </CCardBody>
          <CCardFooter>
              <CButtonGroup class="float-right">
                <CButton color="primary" v-on:click="endEditSection">Ok <CIcon :content="$options.freeSet.cilCheck"/></CButton>
              </CButtonGroup>
            </CCardFooter>
        </CCard>
        <CCard v-if="editor=='skill'">
          <CCardHeader>Edit Skill</CCardHeader>
          <CCardBody>
            <CRow>
              <CCol md="3">
                <CSelect
                label="Skill"
                :value.sync="selectedSkillId"
                :options="allSkillsOptions"
                placeholder="None"></CSelect>
              </CCol>
              <CCol md="3">
                <CInput v-model="newSkillName" label="Name:" v-if="!selectedSkillId">
                </CInput>
              </CCol>
            </CRow>
          </CCardBody>
          <CCardFooter>
              <CButtonGroup class="float-right">
                <CButton color="info" v-on:click="editor='plan'">Cancel <CIcon :content="$options.freeSet.cilX"/></CButton>
                <CButton v-if="selectedSkillId || (newSkillName.length>0 && newSkillName.length<100)" color="primary" v-on:click="endEditSkill">Ok <CIcon :content="$options.freeSet.cilCheck"/></CButton>
              </CButtonGroup>
            </CCardFooter>
        </CCard>
        <CCard v-if="editor=='level'">
          <CCardHeader>Edit Level</CCardHeader>
          <CCardBody>
            <CRow>
              <CCol md="3">
                <CSelect
                label="Skill"
                :value.sync="selectedLevelId"
                :options="allLevelsOptions"
                placeholder="None"></CSelect>
              </CCol>
              <CCol md="3">
                <CInput v-model="newLevelName" label="Name:" v-if="!selectedLevelId">
                </CInput>
              </CCol>
            </CRow>
          </CCardBody>
          <CCardFooter>
              <CButtonGroup class="float-right">
                <CButton color="info" v-on:click="editor='plan'">Cancel <CIcon :content="$options.freeSet.cilX"/></CButton>
                <CButton v-if="selectedLevelId || (newLevelName.length>0 && newLevelName.length<100)" color="primary" v-on:click="endEditLevel">Ok <CIcon :content="$options.freeSet.cilCheck"/></CButton>
              </CButtonGroup>
            </CCardFooter>
        </CCard>
    </div>
</template>

<script>
import { freeSet } from '@coreui/icons'
import VSwatches from 'vue-swatches'
import "vue-swatches/dist/vue-swatches.css"
export default {
  name: 'EditPlan',
  freeSet,
  components: {
    VSwatches 
  },
  props: ['planId'],
  data () {
    return {
        //picklists used by all
        allLevels:[],
        allSkills:[],
        allGroups:[],

        //editor state
        editor:'plan',
        allowRemoveLevel:false,
        allowRemoveSkill:false,
        allowRemoveSection:false,
        
        //plan editor
        plan:{},
        sortedSections:[],
        signOffTable:[],
        levels:[],
        edit:true,

        //section editor
        editSection:{},
        
        //skill editor
        editSkillRow:{},
        selectedSkillId:0,
        newSkillName:'',

        //level editor
        editLevelColumn:{},
        selectedLevelId:0,
        newLevelName:''
    }
  },
  methods: {
        //fetch data
        getPlan(planId) {
          if(planId){
            this.edit=false;
            this.$store.dispatch('loading','Loading...');
            this.$http.get('plan/'+planId)
                .then(response => {
                    console.log(response);
                    this.plan = response.data;
                    this.isLoadComplete();
                    this.getGroups();
                    this.getLevels();
                    this.getSkills();
                }).catch(response => {
                    console.log(response);
                }).finally(response=>this.$store.dispatch('loadingComplete'));
          }
          else{
            this.edit=true;
            this.plan.name="New Plan";
            this.plan.patrolId=this.selectedPatrolId;
            this.addSection(0,1,0,1);
          }
        },
        beginEditSection(section){
          this.editSection = section;
          this.editor='section';

          for(var i=0;i<this.allGroups.length;i++){
            var selected = _.find(this.editSection.groups,{groupId:this.allGroups[i].id}) !=null;
            this.allGroups[i].selected = selected;
          }
        },
        endEditSection(){
          this.editor='plan';
          var selectedGroups = _.filter(this.allGroups,{selected:true});
          
          this.editSection.groups = _.map(selectedGroups,function(g){
            return {groupId:g.id};
          });
        },
        beginEditLevel(sectionLevel){
          this.editLevelColumn = sectionLevel;
          this.selectedLevelId = sectionLevel.level.id;
          this.editor='level';
        },
        endEditLevel(){
          if(this.selectedLevelId){
            var level = _.find(this.allLevels,{id:this.selectedLevelId});
            this.swapLevelIntoPlan(this.editLevelColumn.level.id,this.editLevelColumn.columnIndex,level);
          }
          else{
            this.$store.dispatch('loading','Saving...');
            this.$http.post('plan/levels/create',{patrolId:this.plan.patrolId,name:this.newLevelName})
              .then(response => {
                var newLevel = response.data;
                this.allLevels.push(newLevel);
                this.swapLevelIntoPlan(this.editLevelColumn.level.id,this.editLevelColumn.columnIndex,newLevel);
              }).catch(response=>{
                  console.log(response);
              }).finally(response=>this.$store.dispatch('loadingComplete'));
          }
        },
        beginEditSkill(signOffRow){
          this.editSkillRow = signOffRow;
          this.selectedSkillId = signOffRow.skill.id;
          this.editor='skill';
        },
        endEditSkill(){
          if(this.selectedSkillId){
            var skill = _.find(this.allSkills,{id:this.selectedSkillId});
            this.swapSkillIntoPlan(this.editSkillRow.skill.id,this.editSkillRow.index,skill);
          }
          else{
            this.$store.dispatch('loading','Loading...');
            this.$http.post('plan/skills/create',{patrolId:this.plan.patrolId,name:this.newSkillName})
              .then(response => {
                var newSkill = response.data;
                this.allSkills.push(newSkill);
                this.swapSkillIntoPlan(this.editSkillRow.skill.id,this.editSkillRow.index,newSkill);
              }).catch(response=>{
                  console.log(response);
              }).finally(response=>this.$store.dispatch('loadingComplete'));
          }
        },
        swapSkillIntoPlan(oldSkillId,oldIndex,newSkill){
            //swap the selected skill into the plan
            for(var s=0;s<this.plan.sections.length;s++){
              for(var sk=0;sk<this.plan.sections[s].skills.length;sk++){
                if(this.plan.sections[s].skills[sk].skill.id==oldSkillId 
                  && oldIndex == this.plan.sections[s].skills[sk].rowIndex){
                  this.plan.sections[s].skills[sk].skill = newSkill;
                  this.plan.sections[s].skills[sk].skillId = newSkill.id;
                }
              }
            }

            this.tabelize();
            this.updateEditorButtons();
            //save a new level
            this.editor='plan';
            this.editSkillRow = {};
            this.selectedSkillId = null;
        },
        swapLevelIntoPlan(oldLevelId,oldIndex,newLevel){
            //swap the selected skill into the plan
            for(var s=0;s<this.plan.sections.length;s++){
              for(var sk=0;sk<this.plan.sections[s].levels.length;sk++){
                if(this.plan.sections[s].levels[sk].level.id==oldLevelId 
                  && oldIndex == this.plan.sections[s].levels[sk].columnIndex){
                  this.plan.sections[s].levels[sk].level = newLevel;
                  this.plan.sections[s].levels[sk].levelId = newLevel.id;
                }
              }
            }

            this.tabelize();
            this.updateEditorButtons();
            //save a new level
            this.editor='plan';
            this.editLevelColumn = {};
            this.selectedLevelId = null;
        },
        getGroups() {
          this.$store.dispatch('loading','Loading...');
          this.$http.get('user/groups/'+this.plan.patrolId)
            .then(response => {
              this.allGroups = response.data;
              this.isLoadComplete();
            }).catch(response=>{
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getLevels() {
          this.$store.dispatch('loading','Loading...');
          this.$http.get('plan/levels/'+this.plan.patrolId)
            .then(response => {
              this.allLevels = response.data;
              this.isLoadComplete();
            }).catch(response=>{
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        getSkills() {
          this.$store.dispatch('loading','Loading...');
          this.$http.get('plan/skills/'+this.plan.patrolId)
            .then(response => {
              this.allSkills = response.data;
              this.isLoadComplete();
            }).catch(response=>{
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        isLoadComplete(){
          if(this.allSkills.length>0 && this.allLevels.length>0 && this.allGroups.length>0 && this.plan.sections){
            this.tabelize();
            this.updateEditorButtons();
          }
        },
        createSkill(name,onComplete){
          this.$store.dispatch('loading','Saving...');
          this.$http.post('plan/skills/create',{name:name,patrolId:this.plan.patrolId})
            .then(response => {
              this.allSkills.push(response.data);
              if(onComplete){
                onComplete(response.data);
              }
            }).catch(response=>{
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        createLevel(name,onComplete){
          this.$store.dispatch('loading','Saving...');
          this.$http.post('plan/levels/create',{name:name,patrolId:this.plan.patrolId})
            .then(response => {
              this.allLevels.push(response.data);
              if(onComplete){
                onComplete(response.data);
              }
            }).catch(response=>{
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        

        //utilities
        last(arr){
          return _.last(arr);
        },
        allowSkillAddRemove(signOff){
          console.log(signOff);
          return 
          signOff !=null
          && signOff.section != null
          && signOff.columnIndex == signOff.section.columnIndex + signOff.section.columnCount -1
          && signOff.rowIndex == signOff.section.rowIndex + signOff.section.rowCount -1;
          
        },
        randomColor(){
          var rand = Math.floor(Math.random()*16777215).toString(16);
          return ("#" + rand).toUpperCase();
        },
        
        addSectionRow(index){
          var section = {rowIndex:index,rowCount:1,columnIndex,columnCount:this.levels.length,id:0,name:'New Section',skills:[],levels:[],color:this.randomColor()};
          
          section.skills.push(this.allSkills[0]);

          for(var i=0;i<this.levels.length;i++){
            section.levels.push(this.levels[i]);
          }

          for(var i=0;i<this.plan.sections.length;i++){
            //loop through skills if we need to move anything down
            for(var s=0;s<this.plan.sections[i].skills.length;s++){
                if(this.plan.sections[i].skills[s].rowIndex>=index){
                  this.plan.sections[i].skills[s].rowIndex++;
                }
            }
          }

          this.plan.sections.push(section);
          this.tabelize();
          this.updateEditorButtons();
        },
        //modifiers
        addSection(rowIndex,rowCount,columnIndex,columnCount,sourceSection){
          console.log("addSection",rowIndex,rowCount,columnIndex,columnCount,sourceSection);

          //build up the viewmodel for the new section
          var section = {rowIndex,rowCount,columnIndex,columnCount,id:0,name:'New Section',skills:[],levels:[],color:this.randomColor()};
          
          for(var i=0;i<rowCount;i++){
            //add skills
            var newSkill=null;
            if((!rowCount || rowCount==1) || sourceSection==null){
              newSkill = this.allSkills[0];
            }
            else if(sourceSection!=null){
              for(var ss=0;ss<sourceSection.skills.length;ss++){
                if(sourceSection.skills[ss].rowIndex == rowIndex+i)
                {
                  newSkill = sourceSection.skills[ss].skill;
                }
              }
            }
            section.skills.push({id:0,rowIndex:rowIndex+i,sectionid:0,skill:newSkill});
          }
          for(var i=0;i<columnCount ? columnCount : this.levels.length;i++){
            //add levels
            var newLevel=null;
            if(!columnCount || columnCount==1 || sourceSection==null){
              newLevel = this.allLevels[0];
            }
            else if(sourceSection==null){
              newLevel = this.levels[i];
            }
            else if(sourceSection!=null){
              for(var ss=0;ss<sourceSection.levels.length;ss++){
                if(sourceSection.levels[ss].columnIndex == columnIndex+i)
                {
                  newLevel = sourceSection.levels[ss].level;
                }
              }
            }
            section.levels.push({id:0,columnIndex:columnIndex+i,sectionid:0,level:newLevel});
          }

          //adjust column indexes of other sections that get moved
          for(var i=0;i<this.plan.sections.length;i++){
            //loop through skills if we need to move anything down
            if(rowCount==1){
              for(var s=0;s<this.plan.sections[i].skills.length;s++){
                  if(this.plan.sections[i].skills[s].rowIndex>=rowIndex){
                    this.plan.sections[i].skills[s].rowIndex++;
                  }
              }
            }

            //loop through levels if we need to mvoe anything right
            if(columnCount==1){
              for(var s=0;s<this.plan.sections[i].levels.length;s++){
                  if(this.plan.sections[i].levels[s].columnIndex>=columnIndex){
                    this.plan.sections[i].levels[s].columnIndex++;
                  }
              }
            }
          }

          this.plan.sections.push(section);
          this.tabelize();
          this.updateEditorButtons();
        },
        removeSection(section){
          //TODO: adjust surrounding indexes?

          this.plan.sections = _.pull(this.plan.sections,section);
          this.tabelize();
          this.updateEditorButtons();
        },
        removeSkill(skill,section){
          var removeRowIndex=-1;
          for(var s=0;s<this.plan.sections.length;s++){
            if(section==null || (section!=null && this.plan.sections[s]==section)){
              for(var i=0;i<this.plan.sections[s].skills.length;i++){
                if(this.plan.sections[s].skills[i].skill==skill || (skill.id && this.plan.sections[s].skills[i].skill.id==skill.id)){
                  removeRowIndex = this.plan.sections[s].skills[i].rowIndex;
                  //this.plan.sections[s].skills = 
                  this.plan.sections[s].skills.splice(i,1);
                }
                //only recalc rowindexes if we're removing the skill entirely
                if(section==null 
                && removeRowIndex>-1
                && i< this.plan.sections[s].skills.length
                && removeRowIndex<this.plan.sections[s].skills[i].rowIndex){
                  this.plan.sections[s].skills[i].rowIndex--;
                  //i--;
                }
              }

              if(this.plan.sections[s].skills.length==0){
                _.pull(this.plan.sections,this.plan.sections[s]);
                s--;
              }
            }
          }
          this.pruneEmptySections();
          this.tabelize();
          this.updateEditorButtons();
        },
        pruneEmptySections(){
          for(var s=0;s<this.plan.sections.length;s++){
            if(this.plan.sections[s].skills.length==0 || this.plan.sections[s].levels.length==0){
              this.plan.sections.splice(s,1);
              s--;
            }
          }
        },
        removeLevel(level,section){
          var removeColumnIndex=level.index;

          for(var s=0;s<this.plan.sections.length;s++){
            
            if(!section || (section && this.plan.sections[s]==section)){
              for(var i=0;i<this.plan.sections[s].levels.length;i++){
                if(this.plan.sections[s].levels[i].levelId==level.level.id
                    && this.plan.sections[s].levels[i].columnIndex == removeColumnIndex){
                  this.plan.sections[s].levels.splice(i,1);
                }
              
                //only recalc rowindexes if we're removing the skill entirely
                if(!section 
                && i < this.plan.sections[s].levels.length
                && removeColumnIndex<this.plan.sections[s].levels[i].columnIndex
                ){
                  this.plan.sections[s].levels[i].columnIndex--;
                }
              }
            }
          }
          this.pruneEmptySections();
          this.tabelize();
          this.updateEditorButtons();
          console.log('removed level'+level.index)
        },
        
        //view model buildup
        sortSections(){
            //figure out the size and position of each section
            for(var i=0;i<this.plan.sections.length;i++){
                this.plan.sections[i].rowIndex = _.min(_.map(this.plan.sections[i].skills,x=>x.rowIndex));
                this.plan.sections[i].rowCount = this.plan.sections[i].skills.length;
                this.plan.sections[i].columnIndex = _.min(_.map(this.plan.sections[i].levels,x=>x.columnIndex));
                this.plan.sections[i].columnCount = this.plan.sections[i].levels.length;

                //if(this.plan.sections[i] && !this.plan.sections[i].color){
                  //this.plan.sections[i].color = this.randomColor();
                //}
            }

            //sort by row, then by column
            this.sortedSections = _.orderBy(this.plan.sections,['rowIndex','columnIndex']);
            this.plan.sections = this.sortedSections;
        },
        canonicalizeSkillsAndLevels(){
          //ensure the instances used in the models/viewmodels are the same even in objects loaded from different requests
          for(var s=0;s<this.plan.sections.length;s++){
            for(var ss=0;ss<this.plan.sections[s].skills;ss++){
              this.plan.sections[s].skills[ss].skill = _.find(this.allSkills,{'id':this.plan.sections[s].skills[ss].skillId});
            }
            for(var ss=0;ss<this.plan.sections[s].levels;ss++){
              this.plan.sections[s].levels[ss].level = _.find(this.allLevels,{'id':this.plan.sections[s].levels[ss].levelId});
            }
          }
        },
        tabelize(){
            //convert the list of sections into a 2 dimensional array for easy html formatting
            this.signOffTable=[];
            this.levels=[];

            this.canonicalizeSkillsAndLevels();
            this.sortSections();
            
            for(var i=0;i<this.sortedSections.length;i++){
                for(var s=this.sortedSections[i].rowIndex;s<this.sortedSections[i].rowIndex + this.sortedSections[i].rowCount;s++){
                    //add empty rows until we get to the index needed
                    while(this.signOffTable.length<=s){
                        this.signOffTable.push({skill:null,skillId:0,signOffs:[]});
                    }

                    //if this is a newly added row, set the skill/row header
                    var sectionSkill = _.find(this.sortedSections[i].skills,{rowIndex:s});
                    if(this.signOffTable[s].skill == null && sectionSkill!=null){
                        this.signOffTable[s].skill = sectionSkill.skill;
                        this.signOffTable[s].skillId = sectionSkill.skillId;
                    }

                    //if we're in the first row of a section, make sure to set up a header row
                    if(s==this.sortedSections[i].rowIndex){
                      if(this.signOffTable[s].header == null) {
                          this.signOffTable[s].header = {sections:[]};
                      }
                      this.signOffTable[s].header.sections.push(this.sortedSections[i]);
                      this.signOffTable[s].header.sections = _.orderBy(this.signOffTable[s].header.sections,['columnIndex']);
                    }

                    //if it's the last row of a section, add a footer
                    if(s==this.sortedSections[i].rowIndex+this.sortedSections[i].rowCount-1){
                      if(this.signOffTable[s].footer == null) {
                          this.signOffTable[s].footer = {sections:[]};
                      }
                      this.signOffTable[s].footer.sections.push(this.sortedSections[i]);
                      this.signOffTable[s].footer.sections = _.orderBy(this.signOffTable[s].footer.sections,['columnIndex']);
                    }

                    for(var l=this.sortedSections[i].columnIndex;l<this.sortedSections[i].columnIndex + this.sortedSections[i].columnCount;l++){
                        var sectionLevel = _.find(this.sortedSections[i].levels,{columnIndex:l});

                        while(this.levels.length<=l){
                            this.levels.push(null);
                        }
                        this.levels[l] = sectionLevel;
                        
                        while(this.signOffTable[s].signOffs.length<=l){
                            this.signOffTable[s].signOffs.push({id:i+'-'+l});
                        }

                        //var signature = _.find(this.assignment.signatures,{sectionSkillId:sectionSkill.id,sectionLevelId:sectionLevel.id});
                        
                        this.signOffTable[s].signOffs[l].columnIndex= l;
                        this.signOffTable[s].signOffs[l].rowIndex= s;
                        this.signOffTable[s].signOffs[l].sectionlevel= sectionLevel;
                        this.signOffTable[s].signOffs[l].sectionSkill= sectionSkill;
                        this.signOffTable[s].signOffs[l].section = this.sortedSections[i];

                        //this.signOffTable[s].signOffs[l].signature= signature;
                        //this.signOffTable[s].signOffs[l].currentUserCanSign = this.sortedSections[i].currentUserCanSign;
                        
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

              //pad out footer
              if(this.signOffTable[r].footer!=null){
                //pad out any columns that are empty due to sections that only exist below
                // if(this.signOffTable[r].footer.sections
                // && this.signOffTable[r].footer.sections.length>0
                // && this.signOffTable[r].footer.sections[0].columnIndex>0){
                //   this.signOffTable[r].footer.sections = this.signOffTable[r].footer.sections.splice(0,0,{columnCount:this.signOffTable[r].footer.sections[0].columnIndex});
                // }

                for(var h=0;h<this.signOffTable[r].footer.sections.length;h++){
                  var current = this.signOffTable[r].footer.sections[h];

                  var previous = null;
                  if(h>0){
                    previous = this.signOffTable[r].footer.sections[h-1]
                  }
                  var previousColumn = previous ? previous.columnIndex + previous.columnCount : 0;
                  if(previousColumn!=current.columnIndex){
                    var blank = { columnCount: current.columnIndex - previousColumn, columnIndex:previousColumn };
                    //this.signOffTable[r].footer.sections = 
                    this.signOffTable[r].footer.sections.splice(h,0,blank);
                  }

                  if(h+1<this.signOffTable[r].footer.sections.length){
                      var next = this.signOffTable[r].footer.sections[h+1];

                      if(next.columnIndex - (current.columnIndex + current.columnCount)>1){
                        var blank = { columnCount: next.columnIndex - (current.columnIndex + current.columnCount), columnIndex:current.columnIndex+current.columnCount };
                        this.signOffTable[r].footer.sections.splice(h,0,blank);
                      }
                  }
                  else if(h+1==this.signOffTable[r].footer.sections.length && current.columnIndex + current.columnCount < this.levels.length ){
                    var blank = { columnCount: this.levels.length - (current.columnIndex + current.columnCount), columnIndex:current.columnIndex+current.columnCount };
                    this.signOffTable[r].footer.sections.push(blank);
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
        save(){
          this.$store.dispatch('loading','Loading...');
          this.$http.post('plan/update',this.plan)
            .then(response => {
              this.$router.push({name:'Plans'});
            }).catch(response=>{
                console.log(response);
            }).finally(response=>this.$store.dispatch('loadingComplete'));
        },
        updateEditorButtons(){
          this.allowRemoveLevel=false;
          this.allowRemoveSkill=false;
          this.allowRemoveSection=false;

          var columnIndex=-1;
          var rowIndex=-1;

          for(var i=0;i<this.plan.sections.length;i++){
            if(this.plan.sections[i].levels.length>1){
              this.allowRemoveLevel = true;
            }
            else if(columnIndex==-1){
              columnIndex = this.plan.sections[i].levels[0].columnIndex;
            }
            else if(columnIndex!=this.plan.sections[i].levels[0].columnIndex){
              this.allowRemoveLevel = true;
            }

            if(this.plan.sections[i].skills.length>1){
              this.allowRemoveSkill = true;
            }
            else if(rowIndex==-1){
              rowIndex = this.plan.sections[i].skills[0].rowIndex;
            }
            else if(rowIndex!=this.plan.sections[i].skills[0].rowIndex){
              this.allowRemoveSkill = true;
            }
          }
          this.allowRemoveSection = this.plan.sections.length>1;
        }
  },
  mounted: function(){
      this.getPlan(this.planId);
  },
  computed: {
      allLevelsOptions(){
        var options = _.map(this.allLevels,function(x){return {value:x.id,label:x.name};});
        options.splice(0,0,{value:null,label:'(New...)'});
        return options;
      },
      allSkillsOptions(){
        var options = _.map(this.allSkills,function(x){return {value:x.id,label:x.name};});
        options.splice(0,0,{value:null,label:'(New...)'});
        return options;
      },
      selectedPatrolId: function () {
        return this.$store.state.selectedPatrolId;
      },
      selectedPatrol: function (){
          return this.$store.getters.selectedPatrol;
      }
  }
}
</script>
