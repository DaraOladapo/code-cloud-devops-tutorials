---
layout: post
title:  "Comparing VueJs and ReactJs"
date:   2016-12-12 18:15:05 -0700
categories: technical
---
As a new developer I have found it near impossible to filter through the noise and understand which frameworks are worth learning. People inevitably promote technologies they are currently using because it makes them more marketable, because why would I advertise I just wasted 2 weeks learning something which is little more than a novelty? That being said, VueJs is no novelty.

Cursory research into VueJs turns up recent articles from the launch of VueJs 2.0 demonstrating VueJs is ~33% faster than react. Github trends show that VueJs is still in third behind ReactJsand Angular, which makes sense since Facebook and Google champion those causes. As of August 2016, VueJs was used just less than half as much as ReactJsand Angular. VueJs has made its inroads with strong international adoption. The US based unicorns use ReactJsand Angular, but the Chinese unicorns like Baidu and Alibaba use Vue.

After using VueJs with Electron to make a desktop notes application for an Turing School project, I found VueJs easier to learn than ReactJsfor several reasons. Before I address those differences, it is worth noting that ReactJsis a library not a framework. Some of the benefits of VueJs can be attributed to this key difference.

**First, the structure of VueJs is intuitive**. Each VueJs component has a script tag, where you import all dependencies, maintain the VueJs equivalent of state via the data method, and define inherited properties passed down from other components. You can also create your own methods in the methods object. Each component has a template, which equates to the render method of ReactJscomponents. Passing functions or props down to children has a simple, easy to understand syntax. Finally, each component can carry its own styling which is pretty straightforward.

```
<script>
export default {
  components: {},
  data() {
    return {
      notes: [],
      activeNote: {},
      savedNote: {},
      search: '',
    };
  },
  created() {
    this.fetchNotes();
  },
  methods: {
    addNote(title, body, createdAt, flagged) {
     return database('notes').insert({ title, body, created_at: createdAt, flagged });
  },
};
</script>

<template>
  <div class="app">
    <header-menu :addNote='addNote'>
  </div>
</template>

<style scoped>
  .app {
    width: 100%;
    height: 100%;
    postion: relative;
  }
</style>
```

**Second, the lifecycle events actually make sense**. The chart below demonstrates these event hooks, and implementing them is as simple as inserting a beforeCreate method into the script tag. An example can be found with the created method in the script tag above. The one complaint which can fairly be levied against React in my opinion is that they don’t make using those lifecycle hooks intuitive.

![VueJs Lifecycle Hooks](/assets/vue_lifecycle.png)

**Third, like Angular, it provides training wheels for 2-way data binding.** If you are comfortable using React you know that rendering state has this same affect, but that requires a certain level of comfortability with JavaScript and React.

Vue, like ember, has conventions which are strictly enforced. Because of this, I think you could get by without knowing Javascript and still learn/use Vue. React provides the leeway for developers to use the library however they choose, making it incredibly popular but also varied in its implementation. If React is a tool for front end engineers to build a house, Vue is one of those template homes you find in people farms(those neighborhoods where every house has the exact same layout).

So should you bother to learn Vue? Why not? Compared to other frameworks its quick and easy to adopt. Unfortunately, there are nowhere near the resources available as there are for React, Angular, Ember, Elm, Backbone… you get the point.

Should you choose Vue as part of your tech stack? Assuming you are aspiring for success in the US market, the answer is a firm no. When picking a tech stack you should do so with an eye toward the people who come after you. You should pick a common framework which will make hiring, on-boarding, and therefore progress as a startup come easier.

By learning Vue, it helped me gain a deeper appreciation for the malleability of React. I would recommend spending an afternoon building a basic single page app in Vue to strengthen mental models of modern front end frameworks. Each framework aims to solve the same core problems, and seeing different paths to bypass those problems helps you more deeply understand front end engineering rather than just the syntax for a framework.

Originally posted in [Hackernoon](https://hackernoon.com/) as [Vue vs. React](https://hackernoon.com/vue-vs-react-254a874d74ab).
