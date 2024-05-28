#include <linux/module.h>
#include <linux/kernel.h>
#include <linux/sched.h>
#include <linux/init.h>
#include <linux/sched/signal.h>

MODULE_LICENSE("GPL");
MODULE_AUTHOR("DF");
MODULE_DESCRIPTION("PROCESS DFS MODULE");


void dfs(struct task_struct *task){
	struct task_struct *child;
	struct list_head *list;
	
	printk(KERN_INFO "Name: %-20s State: %ld\tProcess ID: %d\t schedule): %d, Priority (schedule): %d\n", task->comm, task->__state, task->pid, task->prio);

	list_for_each(list, &task->children){
		child = list_entry(list, struct task_struct, sibling);
		
		dfs(child);
	}

}

int init_module(){
	printk(KERN_INFO "PROCESS DFS - MODULE LOADED\n");
	
	struct task_struct *task;
	
	//for_each_process(task){
	//
	//	printk(KERN_INFO "Name: %-20s State: %ld\tProcess ID: %d\n", task->comm, task->__state, task->pid);
	//}	
	dfs(&init_task);

	return 0;
}

void cleanup_module(){
	printk(KERN_INFO "HELLO WORLD - MODULE UNLOADED\n");
}
