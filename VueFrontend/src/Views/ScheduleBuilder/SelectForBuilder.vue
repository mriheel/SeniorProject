<template>
    <div class="list">
        <div v-if="loading" class="loading">
            Loading...
        </div>

        <div class="newSchedule">
            <form id="newScheduleForm">
                <p>
                    Create a new Schedule
                    <input v-model="newScheduleTitle" placeholder="Title" />
                    <button id="newScheduleSubmit" @click="PostSchedule">Create</button>
                </p>
            </form>
        </div>

        <div v-if="list" class="content">
            <table>
                <thead>
                    <tr>
                        <th>id</th>
                        <th>title</th>
                        <th>created</th>
                        <th>modified</th>
                        <th>Owner</th>
                        <th>Edit</th>
                        <th>Delete</th>
                        <th>Collaborators</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="schedule in list" :key="schedule.id">
                        <!--TODO: implement component for schedule details!-->
                        <td>{{ schedule.id }}</td>
                        <td>{{ schedule.title }}</td>
                        <td>{{ schedule.created }}</td>
                        <td>{{ schedule.modified }}</td>
                        <td>{{ schedule.owner }}</td>
                        <td><button @click="onScheduleBuilder(schedule.id, schedule.title, schedule.created, schedule.modified)">Edit</button></td>
                        <td><button @click="deleteSchedule(schedule.id)">Delete</button></td>
                        <td><button @click="editCollaborators(schedule.id)">Collaborators</button></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <router-view />
</template>

<script lang="js">
    import * as $ from 'jquery'
    import router from '../../router'
    import URLS from '../../variables'
    import jwt_decode from 'jwt-decode'
    export default ({
        data() {
            return {
                loading: false,
                list: null,
                newScheduleTitle: "",
                // get user id or other identifier from the router to plug into getList()
                user: jwt_decode(window.sessionStorage.getItem("token")).username
            };
        },
        created() {
            this.getList();
        },
        methods: {
            getList() {
                this.loading = true;
                let requestName = "schedule/getList";
                console.log(requestName);
                $.ajax({
                    // set the HTTP request URL
                    url: `${URLS.api.scheduleBuilder.getList}/${this.user}`,

                    // set the context object to the vue component
                    // this line tells vue to update its components
                    // when the success or error objects complete!
                    // if it's not set, the components don't update!
                    context: this,

                    // HTTP method
                    method: 'GET',

                    // On a successful AJAX request:
                    success: function (data) {
                        this.list = data;
                        this.loading = false;
                        return true;
                    },

                    // On an unsuccessful AJAX request:
                    error: function (error) {
                        // log the error
                        console.log(requestName + "- Error");
                        console.log(error);
                        this.list = null;
                        this.loading = false;
                        return false;
                    }
                });
            },
            PostSchedule() {
                this.loading = true;
                $.ajax({
                    url: `${URLS.api.scheduleBuilder.newSchedule}/${this.user}/${this.newScheduleTitle}`,
                    context: this,
                    processData: true,
                    method: 'POST',
                    success: function (data) {
                        console.log(data);
                        this.loading = false;
                        return;
                    },
                    error: function (error) {
                        console.log(error);
                        this.loading = false;
                        return;
                    }
                });
            },
            onScheduleBuilder(scheduleId, title, created, modified) {
                console.log("To schedule builder @" + scheduleId + "/" + title);
                router.push({
                    name: 'ScheduleBuilder',
                    params: {
                        user: this.user,
                        scheduleId: scheduleId,
                        title: title,
                        created: created,
                        modified: modified,
                    }
                });
            },
            deleteSchedule(scheduleId) {
                let userConfirmed = confirm("Are you sure you want to delete this schedule?");
                if (userConfirmed) {
                    $.ajax({
                        url: `${URLS.api.scheduleBuilder.deleteSchedule}/${this.user}/${scheduleId}`,
                        context: this,
                        method: "DELETE",
                        success: function (data) {
                            console.log(data);
                            this.getList();
                        },
                        error: function (error) {
                            console.log(error);
                        }
                    });
                }
            },
            editCollaborators(scheduleId) {
                console.log("To edit collaborators for @" + scheduleId);
                router.push({
                    name: 'ScheduleCollaborators',
                    params: {
                        user: this.user,
                        scheduleId: scheduleId
                    }
                });
            }
        }
    });
</script>

<style>
    table {
        margin: auto;
    }
    td {
        border-width: 1px;
        border-style: solid;
        border-color: black;
    }
</style>